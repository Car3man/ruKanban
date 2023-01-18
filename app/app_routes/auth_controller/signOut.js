const authHelper = require('../../common/auth-helper');
const responseHelper = require('../../common/response-helper');

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

module.exports = signOut;
