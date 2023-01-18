const util = require('util');
const jwt = require('jsonwebtoken');
const { PrismaClient } = require('@prisma/client');
const { JsonWebTokenError } = require('jsonwebtoken');
const responseHelper = require('./response-helper');

const jwtVerify = util.promisify(jwt.verify);
const prisma = new PrismaClient();

/**
 * Returns date when access token will expire, using current date time
 * @returns {Date}
 */
const getAccessTokenExpiresAt = () => new Date(Date.now() + (1000 * 60 * 5));

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
 * @typedef {Object} VerifyResult
 * @property {Boolean} isValid
 * @property {Object} payload
 * @param {String} token
 * @returns {VerifyResult} token payload
 */
const verifyTokenAsync = async (token) => {
  let payload;
  try {
    payload = await jwtVerify(token, process.env.JWT_SECRET);
  } catch (err) {
    if (err instanceof JsonWebTokenError) {
      return { isValid: false };
    }
    throw err;
  }
  return { isValid: true, payload };
};

/**
 * @param {String} token
 * @returns {Boolean}
 */
const isTokenRevokedAsync = async (token) => await prisma.revoked_tokens.count({
  where: { token },
}) > 0;

/**
 * @param {import('express').Request} req
 * @typedef {Object} AuthorizationState
 * @property {Boolean} isAuthorized
 * @property {String|Undefined} userId
 * @property {String|Undefined} login
 * @property {String|Undefined} accessToken
 * @returns {AuthorizationState}
 */
const getAuthorizationStateAsync = async (req) => {
  const accessToken = getAccessTokenFromHeader(req);

  if (!accessToken) {
    return { isAuthorized: false };
  }

  const verifyResult = await verifyTokenAsync(accessToken);
  if (!verifyResult.isValid) {
    return { isAuthorized: false };
  }

  const isAccessTokenRevoked = await isTokenRevokedAsync(accessToken);

  return {
    isAuthorized: !isAccessTokenRevoked,
    userId: verifyResult.payload.userId,
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
  exp: getAccessTokenExpiresAt().getTime(),
  alg: 'HS256',
  userId: userId.toString(),
  login,
}, process.env.JWT_SECRET);

/**
 * @param {String} accessToken
 * @returns {String}
 */
const createRefreshToken = (accessToken) => jwt.sign({
  iss: process.env.JWT_ISSUER,
  aud: process.env.JWT_AUDIENCE,
  exp: getRefreshTokenExpiresAt().getTime(),
  alg: 'HS256',
  accessToken,
}, process.env.JWT_SECRET);

/**
 * Returns access and refresh tokens
 * @typedef {Object} PairOfTokens
 * @property {String} accessToken
 * @property {String} refreshToken
 * @param {BigInt} userId
 * @param {String} login
 * @returns {PairOfTokens}
 */
const createPairOfTokensAsync = async (userId, login) => {
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
 * @param {String} accessToken
 */
const revokeUserTokensAsync = async (accessToken) => {
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
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 * @param {import('express').NextFunction} next
 * @returns
 */
const unauthorizationRequireAsync = async (req, res, next) => {
  try {
    const { isAuthorized } = await getAuthorizationStateAsync(req);

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
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 * @param {import('express').NextFunction} next
 * @returns
 */
const authorizationRequireAsync = async (req, res, next) => {
  try {
    const {
      isAuthorized, userId, login, accessToken,
    } = await getAuthorizationStateAsync(req);

    if (!isAuthorized) {
      return responseHelper.sendUnauthorized(req, res);
    }

    req.userId = userId;
    req.login = login;
    req.access_token = accessToken;

    return next();
  } catch (err) {
    console.error(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports.verifyTokenAsync = verifyTokenAsync;
module.exports.isTokenRevokedAsync = isTokenRevokedAsync;
module.exports.getAccessTokenFromHeader = getAccessTokenFromHeader;
module.exports.unauthorizationRequireAsync = unauthorizationRequireAsync;
module.exports.authorizationRequireAsync = authorizationRequireAsync;
module.exports.createPairOfTokensAsync = createPairOfTokensAsync;
module.exports.revokeUserTokensAsync = revokeUserTokensAsync;
