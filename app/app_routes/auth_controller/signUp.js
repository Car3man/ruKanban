const { PrismaClient } = require('@prisma/client');
const md5 = require('md5');
const authHelper = require('../../common/auth-helper');
const responseHelper = require('../../common/response-helper');

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

module.exports = signUp;
