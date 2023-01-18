const util = require('util');
const { PrismaClient } = require('@prisma/client');
const md5 = require('md5');
const jwt = require('jsonwebtoken');
const authHelper = require('../common/auth-helper');
const responseHelper = require('../common/response-helper');

const { JsonWebTokenError } = jwt;
const prisma = new PrismaClient();

/**
 * Sign up business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const signUp = async (req, res) => {
  try {
    const {
      login, password, firstName, surName, patronymic,
    } = req.body;

    const isUserExist = await prisma.users.count({
      where: { login },
    }) > 0;

    if (isUserExist) {
      return responseHelper.sendBadRequest(req, res, {
        extended_msg: 'The user with same login exist.',
      });
    }

    const userDataToCreate = {
      login,
      password_hash: md5(password),
      first_name: firstName,
      sur_name: surName,
      patronymic,
      roles: {
        connect: { name: 'user' },
      },
      created_at: new Date(),
    };

    const userId = (await prisma.users.create({
      data: userDataToCreate,
    })).id;

    const { accessToken, refreshToken } = await authHelper.createPairOfTokensAsync(userId, login);

    return responseHelper.sendOk(req, res, {
      accessToken,
      refreshToken,
    });
  } catch (err) {
    console.error(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

/**
 * Sign in business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const signIn = async (req, res) => {
  try {
    const { login, password } = req.body;

    const userData = await prisma.users.findFirst({
      where: { login },
    });

    if (!userData) {
      return responseHelper.sendBadRequest(req, res, {
        extended_msg: 'The login or password is wrong.',
      });
    }

    if (userData.password_hash !== md5(password)) {
      return responseHelper.sendBadRequest(req, res, {
        extended_msg: 'The login or password is wrong.',
      });
    }

    const pairOfTokens = await authHelper.createPairOfTokensAsync(userData.id, login);
    return responseHelper.sendOk(req, res, pairOfTokens);
  } catch (err) {
    console.error(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

/**
 * Sign out business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const signOut = async (req, res) => {
  try {
    const { accessToken } = req;
    await authHelper.revokeUserTokensAsync(accessToken);
    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.error(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

/**
 * Change password business logic
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const changePassword = async (req, res) => {
  try {
    const { login, accessToken } = req;
    const { currentPassword, newPassword } = req.body;

    const userData = await prisma.users.findFirstOrThrow({
      where: { login },
    });

    if (userData.password_hash !== md5(currentPassword)) {
      return responseHelper.sendBadRequest(req, res, {
        extended_msg: 'Provided \'currentPassword\' doesn\'t match actual.',
      });
    }

    await prisma.users.update({
      where: {
        login,
      },
      data: {
        password_hash: md5(newPassword),
      },
    });

    await authHelper.revokeUserTokensAsync(accessToken);

    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.error(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

/**
 * Refresh token business logic
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const refreshTokens = async (req, res) => {
  try {
    const { userId, login } = req;
    const currentAccessToken = req.accessToken;
    const currentRefreshToken = req.body.refreshToken;

    let decodedRefreshToken;
    try {
      const jwtVerify = util.promisify(jwt.verify);
      decodedRefreshToken = await jwtVerify(currentRefreshToken, process.env.JWT_SECRET);
    } catch (err) {
      if (err instanceof JsonWebTokenError) {
        return responseHelper.sendBadRequest(req, res, {
          extended_msg: 'Invalid refresh token',
        });
      }
      throw err;
    }

    const accessTokenFromRefreshToken = decodedRefreshToken.accessToken;
    if (accessTokenFromRefreshToken !== currentAccessToken) {
      return responseHelper.sendBadRequest(req, res, {
        extended_msg: 'Invalid pair access-refresh tokens',
      });
    }

    const { accessToken, refreshToken } = await authHelper.createPairOfTokensAsync(userId, login);

    return responseHelper.sendOk(req, res, {
      accessToken,
      refreshToken,
    });
  } catch (err) {
    console.error(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports.signUp = signUp;
module.exports.signIn = signIn;
module.exports.signOut = signOut;
module.exports.changePassword = changePassword;
module.exports.refreshTokens = refreshTokens;
