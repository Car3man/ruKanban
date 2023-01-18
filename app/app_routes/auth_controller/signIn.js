const { PrismaClient } = require('@prisma/client');
const md5 = require('md5');
const authHelper = require('../../common/auth-helper');
const responseHelper = require('../../common/response-helper');

const prisma = new PrismaClient();

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

module.exports = signIn;
