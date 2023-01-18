const responseHelper = require('../../../common/response-helper');

/**
 * Sign out business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const createWorkspace = async (req, res) => responseHelper.sendOk(req, res, 'createWorkspace');

module.exports = createWorkspace;
