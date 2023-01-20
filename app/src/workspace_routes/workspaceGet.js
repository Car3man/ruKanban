const { responseHelper, workspaceHelper } = require('../common/helpers');
const utils = require('../common/utils');

/**
 * Get workspaces business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const workspaceGet = async (req, res) => {
  try {
    const userId = BigInt(req.userId);
    let { startAt, limit } = req.query;

    startAt = startAt || 0;
    limit = limit || 10;

    const workspaces = await workspaceHelper.getWorkspacesByUserId(userId, startAt, limit);

    const result = workspaces.map((x) => utils.escapeObjectBigInt(x));
    return responseHelper.sendOk(req, res, result);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = workspaceGet;
