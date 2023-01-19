const responseHelper = require('../../../../common/response-helper');

/**
 * Sign out business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const deleteBoard = async (req, res) => responseHelper.sendOk(req, res, 'deleteBoard');

module.exports = deleteBoard;
