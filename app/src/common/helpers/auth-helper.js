const { PrismaClient } = require('@prisma/client');
const jwt = require('jsonwebtoken');
const { JsonWebTokenError } = require('jsonwebtoken');
const util = require('util');

const responseHelper = require('./response-helper');

const jwtVerify = util.promisify(jwt.verify);
const prisma = new PrismaClient();

/**
 * @typedef {Object} ValidationResult
 * @property {Boolean} isValid
 * @property {String|Undefined} details
 * @param {String} login
 * @returns {ValidationResult}
 */
function isUserLoginValid(login) {
  if (typeof login !== 'string') {
    return { isValid: false, details: 'Login type should be a String.' };
  }

  const trimmedLogin = login.trim();
  if (trimmedLogin.length < 5) {
    return { isValid: false, details: 'Login length should be greater than 5.' };
  }

  if (trimmedLogin.length > 36) {
    return { isValid: false, details: 'Login length should be less or equals than 36' };
  }

  return { isValid: true };
}

/**
 * @typedef {Object} ValidationResult
 * @property {Boolean} isValid
 * @property {String|Undefined} details
 * @param {String} password
 * @returns {ValidationResult}
 */
function isUserPasswordValid(password) {
  if (typeof password !== 'string') {
    return { isValid: false, details: 'Password type should be a String.' };
  }

  const trimmedPassword = password.trim();
  if (trimmedPassword.length < 5) {
    return { isValid: false, details: 'Password length should be greater than 5.' };
  }

  if (trimmedPassword.length > 36) {
    return { isValid: false, details: 'Password length should be less or equals than 36' };
  }

  return { isValid: true };
}

/**
 * Returns date when access token will expire, using current date time
 * @returns {Date}
 */
const getAccessTokenExpiresAt = () => new Date(Date.now() + (1000 * 60 * 60 * 5)); // TODO: decrease to 5 minutes

/**
 * Returns date when refresh token will expire, using current date time
 * @returns {Date}
 */
const getRefreshTokenExpiresAt = () => new Date(Date.now() + (1000 * 60 * 60 * 24 * 30 * 3));

/**
 * @param {import('express').Request} req
 * @returns {String}
 */
const getAccessTokenFromHeader = (req) => {
  const authorizationHeader = req.headers.authorization;

  if (!authorizationHeader) {
    return null;
  }

  const authorizationHeaderParts = authorizationHeader.split(' ');
  const accessToken = authorizationHeaderParts.length > 1 ? authorizationHeaderParts[1] : null;
  return accessToken;
};

/**
 * Verify JWT token
 * @async
 * @typedef {Object} VerifyResult
 * @property {Boolean} isValid
 * @property {Object} payload
 * @param {String} token
 * @returns {VerifyResult} token payload
 */
const verifyToken = async (token) => {
  let payload;
  try {
    payload = await jwtVerify(token, global.jwtSecret);
  } catch (err) {
    if (err instanceof JsonWebTokenError) {
      return { isValid: false };
    }
    throw err;
  }
  return { isValid: true, payload };
};

/**
 * @async
 * @param {String} token
 * @returns {Boolean}
 */
const isTokenRevoked = async (token) => await prisma.revoked_tokens.count({
  where: { token },
}) > 0;

/**
 * @async
 * @typedef {Object} AuthorizationState
 * @property {Boolean} isAuthorized
 * @property {String|Undefined} userId
 * @property {String|Undefined} login
 * @property {String|Undefined} accessToken
 * @param {import('express').Request} req
 * @returns {AuthorizationState}
 */
const getAuthorizationState = async (req) => {
  const accessToken = getAccessTokenFromHeader(req);

  if (!accessToken) {
    return { isAuthorized: false };
  }

  const verifyResult = await verifyToken(accessToken);
  if (!verifyResult.isValid) {
    return { isAuthorized: false };
  }

  const isAccessTokenRevoked = await isTokenRevoked(accessToken);

  return {
    isAuthorized: !isAccessTokenRevoked,
    userId: BigInt(verifyResult.payload.userId),
    login: verifyResult.payload.login,
    accessToken,
  };
};

/**
 * @param {BigInt} userId
 * @param {String} login
 * @returns {String}
 */
const createAccessToken = (userId, login) => jwt.sign({
  iss: process.env.JWT_ISSUER,
  aud: process.env.JWT_AUDIENCE,
  exp: Math.floor(getAccessTokenExpiresAt().getTime() / 1000),
  alg: 'HS256',
  userId: userId.toString(),
  login,
}, global.jwtSecret);

/**
 * @param {String} accessToken
 * @returns {String}
 */
const createRefreshToken = (accessToken) => jwt.sign({
  iss: process.env.JWT_ISSUER,
  aud: process.env.JWT_AUDIENCE,
  exp: Math.floor(getRefreshTokenExpiresAt().getTime() / 1000),
  alg: 'HS256',
  accessToken,
}, global.jwtSecret);

/**
 * Returns access and refresh tokens
 * @async
 * @typedef {Object} PairOfTokens
 * @property {String} accessToken
 * @property {String} refreshToken
 * @param {BigInt} userId
 * @param {String} login
 * @returns {PairOfTokens}
 */
const createPairOfTokens = async (userId, login) => {
  const accessToken = createAccessToken(userId, login);
  const refreshToken = createRefreshToken(accessToken);

  const tokensCreatedAt = new Date();
  const refreshTokenExpiresAt = getRefreshTokenExpiresAt();

  await prisma.refresh_tokens.create({
    data: {
      token: refreshToken,
      users: {
        connect: { login },
      },
      created_at: tokensCreatedAt,
      expires_at: refreshTokenExpiresAt,
    },
  });

  return { accessToken, refreshToken };
};

/**
 * Revoke access and refresh user tokens
 * @async
 * @param {String} accessToken
 */
const revokeUserTokens = async (accessToken) => {
  const { userId } = jwt.decode(accessToken);

  const refreshToken = (await prisma.refresh_tokens.findFirst({
    where: {
      user_id: BigInt(userId),
    },
    select: {
      token: true,
    },
  })).token;

  await prisma.refresh_tokens.delete({
    where: {
      token: refreshToken,
    },
  });

  await prisma.revoked_tokens.createMany({
    data: [
      { token: accessToken, revoked_at: new Date() },
      { token: refreshToken, revoked_at: new Date() },
    ],
  });
};

/**
 * Express middleware which checks is user unauthorized (safety)
 * @async
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 * @param {import('express').NextFunction} next
 * @returns
 */
const unauthorizationRequire = async (req, res, next) => {
  try {
    const { isAuthorized } = await getAuthorizationState(req);

    if (isAuthorized) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: `Sign out before use ${req.path} method`,
      });
    }

    return next();
  } catch (err) {
    console.error(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

/**
 * Express middleware which checks is user authorized (safety)
 * @async
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 * @param {import('express').NextFunction} next
 * @returns
 */
const authorizationRequire = async (req, res, next) => {
  try {
    const {
      isAuthorized, userId, login, accessToken,
    } = await getAuthorizationState(req);

    if (!isAuthorized) {
      return responseHelper.sendUnauthorized(req, res);
    }

    req.userId = userId;
    req.login = login;
    req.accessToken = accessToken;

    return next();
  } catch (err) {
    console.error(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports.isUserLoginValid = isUserLoginValid;
module.exports.isUserPasswordValid = isUserPasswordValid;
module.exports.verifyToken = verifyToken;
module.exports.isTokenRevoked = isTokenRevoked;
module.exports.getAccessTokenFromHeader = getAccessTokenFromHeader;
module.exports.unauthorizationRequire = unauthorizationRequire;
module.exports.authorizationRequire = authorizationRequire;
module.exports.createPairOfTokens = createPairOfTokens;
module.exports.revokeUserTokens = revokeUserTokens;
