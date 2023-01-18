const { PrismaClient } = require('@prisma/client');
const workspaceHelper = require('../../../common/workspace-helper');
const responseHelper = require('../../../common/response-helper');

const prisma = new PrismaClient();

/**
 * Sign out business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const deleteWorkspace = async (req, res) => {
  try {
    const userId = BigInt(req.userId);
    const workspaceId = BigInt(req.params.id);

    const isAllow = await workspaceHelper.isUserCanDeleteWorkspaceAsync(userId, workspaceId);
    if (!isAllow) {
      return responseHelper.sendForbidden(req, res);
    }

    await prisma.user_workspace.delete({
      where: {
        user_id_workspace_id: {
          user_id: userId,
          workspace_id: workspaceId,
        },
      },
    });
    await prisma.workspaces.delete({
      where: {
        id: workspaceId,
      },
    });
    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = deleteWorkspace;
