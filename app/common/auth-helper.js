const jwt = require('jsonwebtoken');
const { PrismaClient } = require('@prisma/client');
const responseHelper = require('./response-helper');

const prisma = new PrismaClient();

const getAuthorizationStatus = async (req) => {
  const authorizationHeader = req.headers.authorization;

  if (!authorizationHeader) {
    return { isAuthorized: false };
  }

  const token = authorizationHeader.split(' ')[1];
  const jwtVerifyPromise = new Promise((resolve, reject) => {
    jwt.verify(token, process.env.JWT_SECRET, async (verifyErr, { login }) => {
      if (verifyErr) {
        console.log(verifyErr);
        return reject(verifyErr);
      }

      try {
        const isRevoked = await prisma.revoked_tokens.count({
          where: { token },
        }) > 0;
        return resolve({ isAuthorized: !isRevoked, login });
      } catch (isRevokeErr) {
        console.log(isRevokeErr);
        return { isAuthorized: false };
      }
    });
  });

  try {
    const jwtVerifyResult = await jwtVerifyPromise;
    return { ...jwtVerifyResult, accessToken: token };
  } catch {
    return { isAuthorized: false };
  }
};

module.exports.unauthorizationRequireMiddleware = async (req, res, next) => {
  const status = await getAuthorizationStatus(req);

  if (status.isAuthorized) {
    return responseHelper.sendBadRequest(req, res, {
      extended_msg: `Sign out before use ${req.path} method`,
    });
  }

  return next();
};

module.exports.authorizationRequireMiddleware = async (req, res, next) => {
  const status = await getAuthorizationStatus(req);

  if (!status.isAuthorized) {
    return responseHelper.sendUnauthorized(req, res);
  }

  req.login = status.login;
  req.accessToken = status.accessToken;

  return next();
};

module.exports.createAccessToken = (userId, login) => jwt.sign({
  iss: process.env.JWT_ISSUER,
  aud: process.env.JWT_AUDIENCE,
  exp: Date.now() + (1000 * 60 * 60 * 5),
  alg: 'HS256',
  userId: userId.toString(),
  login,
}, process.env.JWT_SECRET);

module.exports.createRefreshToken = (accessToken, expiresAt) => jwt.sign({
  iss: process.env.JWT_ISSUER,
  aud: process.env.JWT_AUDIENCE,
  exp: expiresAt,
  alg: 'HS256',
  accessToken,
}, process.env.JWT_SECRET);

module.exports.revokeUserTokens = async (accessToken) => {
  const { userId } = jwt.decode(accessToken);

  const refreshToken = (await prisma.refresh_tokens.findFirst({
    where: {
      user_id: BigInt(userId),
    },
    select: {
      token: true,
    },
  })).token;

  console.log(accessToken);
  console.log(refreshToken);

  await prisma.revoked_tokens.createMany({
    data: [
      { token: accessToken, revoked_at: new Date() },
      { token: refreshToken, revoked_at: new Date() },
    ],
  });
};
