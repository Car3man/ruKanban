const { PrismaClient } = require('@prisma/client');
const md5 = require('md5');

const { authHelper, responseHelper } = require('../common/helpers');

const prisma = new PrismaClient();

/**
 * Sign up business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const authSignUp = async (req, res) => {
  try {
    const { login } = req.body;
    const { password } = req.body;
    const firstName = req.body.first_name;
    const surName = req.body.sur_name;
    const { patronymic } = req.body;

    const isUserExist = await prisma.users.count({
      where: { login },
    }) > 0;

    if (isUserExist) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: 'The user with same login exist.',
      });
    }

    const loginValidationResult = authHelper.isUserLoginValid(login);
    if (!loginValidationResult.isValid) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: loginValidationResult.details,
      });
    }

    const passwordValidationResult = authHelper.isUserPasswordValid(password);
    if (!passwordValidationResult.isValid) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: passwordValidationResult.details,
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

module.exports = authSignUp;
