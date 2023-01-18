/**
 * Change password endpoint.
 * @module /routes/auth/changePassword
 */

const { PrismaClient } = require('@prisma/client');
const md5 = require('md5');
const authHelper = require('../../common/auth-helper');
const responseHelper = require('../../common/response-helper');

const prisma = new PrismaClient();

/**
 * Change password business logic
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const changePassword = async (req, res) => {
  try {
    const { login, accessToken } = req;
    const { currentPassword, newPassword } = req.body;

    console.log(`login: ${login}`);

    const userData = await prisma.users.findFirstOrThrow({
      where: { login },
    });

    console.log(`userData: ${userData}`);

    if (userData.password_hash !== md5(currentPassword)) {
      return responseHelper.sendBadRequest(req, res, {
        extended_msg: 'Provided \'currentPassword\' doesn\'t match actual.',
      });
    }

    console.log(`login: ${login}`);

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

module.exports = changePassword;
