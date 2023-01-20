const { workspaceHelper, responseHelper } = require('../common/helpers');

/**
 * Delete workspace business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const workspaceDelete = async (req, res) => {
  try {
    const { userId } = req;
    const workspaceId = BigInt(req.query.workspace_id);

    const workspace = await workspaceHelper.getWorkspaceById(workspaceId);

    if (!workspace) {
      return responseHelper.sendNotFound(req, res);
    }

    const isAllowToDelete = await workspaceHelper.isUserWorkspaceOwner(userId, workspaceId);
    if (!isAllowToDelete) {
      return responseHelper.sendForbidden(req, res);
    }

    await workspaceHelper.deleteWorkspace(workspaceId);

    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = workspaceDelete;
