const jwt = require('jsonwebtoken');

const { authHelper, responseHelper } = require('../common/helpers');

/**
 * Refresh token business logic
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const authRefreshTokens = async (req, res) => {
  try {
    const currentAccessToken = authHelper.getAccessTokenFromHeader(req);
    const currentRefreshToken = req.body.refresh_token;

    const isAccessTokenRevoked = await authHelper.isTokenRevoked(currentAccessToken);
    if (isAccessTokenRevoked) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: 'The access token is revoked',
      });
    }

    const verifyRefreshTokenResult = await authHelper.verifyToken(currentRefreshToken);
    if (!verifyRefreshTokenResult.isValid) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: 'The refresh token invalid',
      });
    }

    const accessTokenFromRefreshToken = verifyRefreshTokenResult.payload.accessToken;
    if (accessTokenFromRefreshToken !== currentAccessToken) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: 'Invalid pair access-refresh tokens',
      });
    }

    await authHelper.revokeUserTokens(currentAccessToken);

    const { userId, login } = jwt.decode(currentAccessToken);
    const { accessToken, refreshToken } = await authHelper.createPairOfTokens(userId, login);

    return responseHelper.sendOk(req, res, {
      access_token: accessToken,
      refresh_token: refreshToken,
    });
  } catch (err) {
    console.error(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = authRefreshTokens;
