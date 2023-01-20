const { PrismaClient } = require('@prisma/client');
const md5 = require('md5');

const { authHelper, responseHelper } = require('../common/helpers');

const prisma = new PrismaClient();

/**
 * Change password business logic
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const authChangePassword = async (req, res) => {
  try {
    const { login } = req;
    const { accessToken } = req;
    const currentPassword = req.body.current_password;
    const newPassword = req.body.new_password;

    const passwordValidationResult = authHelper.isUserPasswordValid(newPassword);
    if (!passwordValidationResult.isValid) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: passwordValidationResult.details,
      });
    }

    const userData = await prisma.users.findFirstOrThrow({
      where: { login },
      select: { password_hash: true },
    });

    if (userData.password_hash !== md5(currentPassword)) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: 'Provided \'currentPassword\' doesn\'t match actual.',
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

    await authHelper.revokeUserTokens(accessToken);

    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.error(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = authChangePassword;
