const { authHelper, responseHelper } = require('../common/helpers');

/**
 * Sign out business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const authSignOut = async (req, res) => {
  try {
    const { accessToken } = req;

    await authHelper.revokeUserTokens(accessToken);

    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.error(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = authSignOut;
