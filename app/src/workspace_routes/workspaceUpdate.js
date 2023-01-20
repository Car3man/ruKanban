const { PrismaClient } = require('@prisma/client');

const { workspaceHelper, responseHelper } = require('../common/helpers');
const UpdateWorkspaceError = require('../common/errors/UpdateWorkspaceError');

const prisma = new PrismaClient();

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
    const usersToAdd = req.body.usersToAdd ? req.body.usersToAdd.map((x) => BigInt(x)) : undefined;
    const usersToDelete = req.body.usersToDelete ? req.body.usersToDelete.map((x) => BigInt(x)) : undefined;

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

    const workspaceOwnerId = (await prisma.user_workspace.findFirst({
      where: {
        workspace_id: workspaceId,
        workspace_roles: {
          is: {
            name: 'owner',
          },
        },
      },
    })).user_id;

    if (usersToDelete && usersToDelete.includes(workspaceOwnerId)) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: 'Impossible delete yourself from workspace.',
      });
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
