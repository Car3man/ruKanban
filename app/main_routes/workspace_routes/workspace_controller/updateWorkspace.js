const { PrismaClient } = require('@prisma/client');
const workspaceHelper = require('../../../common/workspace-helper');
const responseHelper = require('../../../common/response-helper');

const prisma = new PrismaClient();

/**
 * Sign out business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const updateWorkspace = async (req, res) => {
  try {
    const userId = BigInt(req.userId);
    const workspaceId = BigInt(req.params.id);
    const newWorkspaceName = req.body.name;
    const usersToAdd = req.body.usersToAdd ? req.body.usersToAdd.map((x) => BigInt(x)) : undefined;
    const usersToDelete = req.body.usersToDelete ? req.body.usersToDelete.map((x) => BigInt(x)) : undefined;

    const isAllow = await workspaceHelper.isUserCanUpdateWorkspaceAsync(userId, workspaceId);
    if (!isAllow) {
      return responseHelper.sendForbidden(req, res);
    }

    const updateQueries = [];

    if (newWorkspaceName) {
      const nameValidationResult = workspaceHelper.isWorkspaceNameValid(newWorkspaceName);
      if (!nameValidationResult.isValid) {
        return responseHelper.sendBadRequest(req, res, {
          error_msg: nameValidationResult.details,
        });
      }

      const updateNameQuery = prisma.workspaces.update({
        where: { id: workspaceId },
        data: { name: newWorkspaceName.trim() },
      });
      updateQueries.push(updateNameQuery);
    }

    if (usersToAdd || usersToDelete) {
      const workspaceUsers = (await prisma.user_workspace.findMany({
        where: {
          workspace_id: workspaceId,
        },
        select: { user_id: true },
      })).map((x) => x.user_id);

      if (usersToAdd) {
        /* eslint-disable-next-line */
        for (const userToAdd of usersToAdd) {
          if (workspaceUsers.includes(userToAdd)) {
            return responseHelper.sendBadRequest(req, res, {
              error_msg: `User with user_id = '${userToAdd}' already added.`,
            });
          }
        }

        const workspaceRoleId = (await prisma.workspace_roles.findFirstOrThrow({
          where: {
            name: 'user',
          },
          select: { id: true },
        })).id;

        const userWorkspaceRecordsToInsert = [];
        /* eslint-disable-next-line */
        for (const userToAdd of usersToAdd) {
          userWorkspaceRecordsToInsert.push({
            user_id: userToAdd,
            workspace_id: workspaceId,
            workspace_role_id: workspaceRoleId,
          });
        }

        const addUsersQuery = prisma.user_workspace.createMany({
          data: userWorkspaceRecordsToInsert,
        });
        updateQueries.push(addUsersQuery);
      }

      if (usersToDelete) {
        if (usersToDelete.includes(userId.toString())) {
          return responseHelper.sendBadRequest(req, res, {
            error_msg: 'Impossible delete yourself from workspace.',
          });
        }

        /* eslint-disable-next-line */
        for (const userToDelete of usersToDelete) {
          if (!workspaceUsers.includes(userToDelete)) {
            return responseHelper.sendBadRequest(req, res, {
              error_msg: `User with user_id = '${userToDelete}' there is no in the list of users.`,
            });
          }
        }

        const deleteUsersQuery = prisma.user_workspace.deleteMany({
          where: {
            user_id: { in: usersToDelete },
            workspace_id: workspaceId,
          },
        });
        updateQueries.push(deleteUsersQuery);
      }
    }

    await prisma.$transaction(updateQueries);
    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = updateWorkspace;
