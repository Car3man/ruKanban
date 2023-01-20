const { workspaceHelper, responseHelper } = require('../common/helpers');
const utils = require('../common/utils');

/**
 * Get workspace by id business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const workspaceGetById = async (req, res) => {
  try {
    const { userId } = req;
    const workspaceId = BigInt(req.query.workspace_id);

    const workspace = await workspaceHelper.getWorkspaceById(workspaceId);

    if (!workspace) {
      return responseHelper.sendNotFound(req, res);
    }

    const isAllowToGet = await workspaceHelper.isUserWorkspaceParticipant(userId, workspaceId);
    if (!isAllowToGet) {
      return responseHelper.sendForbidden(req, res);
    }

    const result = utils.escapeObjectBigInt(workspace);
    return responseHelper.sendOk(req, res, result);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = workspaceGetById;
