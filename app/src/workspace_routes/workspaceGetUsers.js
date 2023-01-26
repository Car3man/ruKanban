const { responseHelper, workspaceHelper } = require('../common/helpers');
const utils = require('../common/utils');

/**
 * Get workspace users business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const workspaceGetUsers = async (req, res) => {
  try {
    const userId = BigInt(req.userId);
    const workspaceId = BigInt(req.query.workspace_id);
    let { startAt, limit } = req.query;

    startAt = startAt || 0;
    limit = limit || 25;

    const workspace = await workspaceHelper.getWorkspaceById(workspaceId);

    if (!workspace) {
      return responseHelper.sendNotFound(req, res);
    }

    const isAllowToGet = await workspaceHelper.isUserWorkspaceParticipant(userId, workspaceId);
    if (!isAllowToGet) {
      return responseHelper.sendForbidden(req, res);
    }

    const users = await workspaceHelper.getWorkspaceUsers(workspaceId, startAt, limit);

    const result = {
      users: users.map((x) => utils.escapeObjectBigInt(x)),
    };
    return responseHelper.sendOk(req, res, result);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = workspaceGetUsers;
