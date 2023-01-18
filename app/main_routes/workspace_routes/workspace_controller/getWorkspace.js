const responseHelper = require('../../../common/response-helper');

/**
 * Sign out business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const getWorkspace = async (req, res) => responseHelper.sendOk(req, res, 'getWorkspace');

module.exports = getWorkspace;
