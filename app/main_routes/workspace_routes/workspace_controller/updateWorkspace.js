const responseHelper = require('../../../common/response-helper');

/**
 * Sign out business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const updateWorkspace = async (req, res) => responseHelper.sendOk(req, res, 'updateWorkspace');

module.exports = updateWorkspace;
