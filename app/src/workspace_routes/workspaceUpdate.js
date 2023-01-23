const { workspaceHelper, responseHelper } = require('../common/helpers');
const UpdateWorkspaceError = require('../common/errors/UpdateWorkspaceError');

/**
 * Update workspace business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const workspaceUpdate = async (req, res) => {
  try {
    const { userId } = req;
    const workspaceId = BigInt(req.query.workspace_id);
    const newWorkspaceName = req.body.name;
    const usersToAdd = req.body.users_to_add;
    const usersToDelete = req.body.users_to_delete;

    const workspace = await workspaceHelper.getWorkspaceById(workspaceId);

    if (!workspace) {
      return responseHelper.sendNotFound(req, res);
    }

    const isAllowToUpdate = await workspaceHelper.isUserWorkspaceOwner(userId, workspaceId);
    if (!isAllowToUpdate) {
      return responseHelper.sendForbidden(req, res);
    }

    if (newWorkspaceName) {
      const nameValidationResult = workspaceHelper.isWorkspaceNameValid(newWorkspaceName);
      if (!nameValidationResult.isValid) {
        return responseHelper.sendBadRequest(req, res, {
          error_msg: nameValidationResult.details,
        });
      }
    }

    try {
      await workspaceHelper.updateWorkspace(workspaceId, newWorkspaceName, usersToAdd, usersToDelete);
    } catch (err) {
      if (err instanceof UpdateWorkspaceError) {
        return responseHelper.sendBadRequest(req, res, {
          error_msg: err.message,
        });
      }
      throw err;
    }

    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = workspaceUpdate;
