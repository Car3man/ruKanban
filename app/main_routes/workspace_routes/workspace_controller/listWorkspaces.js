const responseHelper = require('../../../common/response-helper');

/**
 * Sign out business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const listWorkspaces = async (req, res) => responseHelper.sendOk(req, res, 'listWorkspaces');

module.exports = listWorkspaces;
