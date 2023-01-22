/* eslint-disable no-restricted-syntax */
/* eslint-disable no-await-in-loop */

const { PrismaClient } = require('@prisma/client');

const boardHelper = require('./board-helper');
const UpdateWorkspaceError = require('../errors/UpdateWorkspaceError');

const prisma = new PrismaClient();

/**
 * @typedef {Object} ValidationResult
 * @property {Boolean} isValid
 * @property {String|Undefined} details
 * @param {String} name
 * @returns {ValidationResult}
 */
function isWorkspaceNameValid(name) {
  if (typeof name !== 'string') {
    return { isValid: false, details: 'Name type should be a String.' };
  }

  const trimmedName = name.trim();
  if (trimmedName.length < 5) {
    return { isValid: false, details: 'Name length should be greater than 5.' };
  }

  if (trimmedName.length > 36) {
    return { isValid: false, details: 'Name length should be less or equals than 36' };
  }

  return { isValid: true };
}

/**
 * @param {BigInt} id
 * @returns {Array<import('@prisma/client').user_workspace}
 */
async function getWorkspaceUsers(id, skip, take) {
  return prisma.user_workspace.findMany({
    where: { workspace_id: id },
    select: { user_id: true },
    skip,
    take,
  });
}

/**
 * @param {BigInt} id
 * @returns {Array<BigInt>}
 */
async function getWorkspaceUserIds(id) {
  return (await prisma.user_workspace.findMany({
    where: { workspace_id: id },
    select: { user_id: true },
  })).map((x) => x.user_id);
}

/**
 * @param {BigInt} userId
 * @param {BigInt} workspaceId
 * @returns {Boolean}
 */
async function isUserWorkspaceParticipant(userId, workspaceId) {
  return await prisma.user_workspace.count({
    where: {
      user_id: userId,
      workspace_id: workspaceId,
    },
  }) > 0;
}

/**
 * @param {BigInt} userId
 * @param {BigInt} workspaceId
 * @returns {Boolean}
 */
async function isUserWorkspaceOwner(userId, workspaceId) {
  const userWorkspace = await prisma.user_workspace.findFirst({
    where: {
      user_id: userId,
      workspace_id: workspaceId,
    },
    select: {
      workspace_role_id: true,
    },
  });

  if (!userWorkspace) {
    return false;
  }

  const userWorkspaceRoleId = userWorkspace.workspace_role_id;

  const userWorkspaceRoleName = (await prisma.workspace_roles.findFirstOrThrow({
    where: {
      id: userWorkspaceRoleId,
    },
    select: { name: true },
  })).name;

  return userWorkspaceRoleName === 'owner';
}

/**
 * @async
 * @typedef {Object} WorkspaceUser
 * @property {BigInt} userId
 * @property {String} roleName
 * @param {String} name
 * @param {Array<WorkspaceUser>} workspaceUsers
 * @returns {import('@prisma/client').workspaces}
 */
async function createWorkspace(name, workspaceUsers) {
  return prisma.$transaction(async (tx) => {
    const workspace = await tx.workspaces.create({
      data: {
        name,
        created_at: new Date(),
      },
    });

    for (const workspaceUser of workspaceUsers) {
      const workspaceRoleId = (await tx.workspace_roles.findFirst({
        where: {
          name: workspaceUser.roleName,
        },
        select: {
          id: true,
        },
      })).id;

      await tx.user_workspace.create({
        data: {
          user_id: workspaceUser.userId,
          workspace_id: workspace.id,
          workspace_role_id: workspaceRoleId,
        },
      });
    }

    return workspace;
  });
}

/**
 * @async
 * @param {BigInt} id
 * @param {String|Undefined} newName
 * @param {Array<BigInt>|Undefined} usersToAdd
 * @param {Array<BigInt>|Undefined} usersToDelete
 */
async function updateWorkspace(id, newName, usersToAdd, usersToDelete) {
  return prisma.$transaction(async (tx) => {
    if (newName) {
      await tx.workspaces.update({
        where: { id },
        data: { name: newName },
      });
    }

    if (usersToAdd || usersToDelete) {
      const workspaceUserIds = (await tx.user_workspace.findMany({
        where: { workspace_id: id },
        select: { user_id: true },
      })).map((x) => x.user_id);

      if (usersToAdd && usersToAdd.length > 0) {
        for (const userToAdd of usersToAdd) {
          if (workspaceUserIds.includes(userToAdd)) {
            throw new UpdateWorkspaceError(`User with user_id = '${userToAdd}' already added.`);
          }
        }

        const userWorkspaceRoleId = (await prisma.workspace_roles.findFirstOrThrow({
          where: { name: 'user' },
          select: { id: true },
        })).id;

        const userWorkspaceRecordsToInsert = [];
        for (const userToAdd of usersToAdd) {
          userWorkspaceRecordsToInsert.push({
            user_id: userToAdd,
            workspace_id: id,
            workspace_role_id: userWorkspaceRoleId,
          });
        }

        await prisma.user_workspace.createMany({
          data: userWorkspaceRecordsToInsert,
        });
      }

      if (usersToDelete && usersToDelete.length > 0) {
        for (const userToDelete of usersToDelete) {
          if (!workspaceUserIds.includes(userToDelete)) {
            throw new UpdateWorkspaceError(`User with user_id = '${userToDelete}' there is no in the list of users.`);
          }
        }

        await prisma.user_workspace.deleteMany({
          where: {
            user_id: { in: usersToDelete },
            workspace_id: id,
          },
        });
      }
    }
  });
}

/**
 * @async
 * @param {BigInt} id
 */
async function deleteWorkspace(id) {
  return prisma.$transaction(async (tx) => {
    await tx.user_workspace.deleteMany({
      where: {
        workspace_id: id,
      },
    });

    await tx.workspaces.delete({
      where: {
        id,
      },
    });
  });
}

/**
 * @async
 * @param {BigInt} userId
 * @param {Number} skip
 * @param {Number} take
 * @returns {import('@prisma/client').workspaces}
 */
async function getWorkspacesByUserId(userId, skip, take) {
  const workspaceIds = (await prisma.user_workspace.findMany({
    where: {
      user_id: userId,
    },
    select: {
      workspace_id: true,
    },
    skip,
    take,
  })).map((x) => x.workspace_id);

  return prisma.workspaces.findMany({
    where: {
      id: { in: workspaceIds },
    },
    select: {
      id: true,
      name: true,
      created_at: true,
    },
  });
}

/**
 * @async
 * @param {BigInt} workspaceId
 * @returns {import('@prisma/client').workspaces}
 */
async function getWorkspaceById(workspaceId) {
  return prisma.workspaces.findFirst({
    where: {
      id: workspaceId,
    },
    select: {
      id: true,
      name: true,
      created_at: true,
    },
  });
}

/**
 * @async
 * @param {BigInt} boardId
 * @returns {import('@prisma/client').workspaces}
 */
async function getWorkspaceByBoardId(boardId) {
  const workspaceId = (await prisma.boards.findFirst({
    where: {
      id: boardId,
    },
    select: {
      workspace_id: true,
    },
  })).workspace_id;

  return prisma.workspaces.findFirst({
    where: {
      id: workspaceId,
    },
    select: {
      id: true,
      name: true,
      created_at: true,
    },
  });
}

/**
 * @async
 * @param {BigInt} columnId
 * @returns {import('@prisma/client').workspaces}
 */
async function getWorkspaceByColumnId(columnId) {
  const workspaceId = (await boardHelper.getBoardByColumnId(columnId)).workspace_id;

  return prisma.workspaces.findFirst({
    where: {
      id: workspaceId,
    },
    select: {
      id: true,
      name: true,
      created_at: true,
    },
  });
}

/**
 * @async
 * @param {BigInt} ticketId
 * @returns {import('@prisma/client').workspaces}
 */
async function getWorkspaceByTicketId(ticketId) {
  const workspaceId = (await boardHelper.getBoardByTicketId(ticketId)).workspace_id;

  return prisma.workspaces.findFirst({
    where: {
      id: workspaceId,
    },
    select: {
      id: true,
      name: true,
      created_at: true,
    },
  });
}

module.exports.isWorkspaceNameValid = isWorkspaceNameValid;
module.exports.getWorkspaceUsers = getWorkspaceUsers;
module.exports.getWorkspaceUserIds = getWorkspaceUserIds;
module.exports.isUserWorkspaceParticipant = isUserWorkspaceParticipant;
module.exports.isUserWorkspaceOwner = isUserWorkspaceOwner;
module.exports.createWorkspace = createWorkspace;
module.exports.updateWorkspace = updateWorkspace;
module.exports.deleteWorkspace = deleteWorkspace;
module.exports.getWorkspacesByUserId = getWorkspacesByUserId;
module.exports.getWorkspaceById = getWorkspaceById;
module.exports.getWorkspaceByBoardId = getWorkspaceByBoardId;
module.exports.getWorkspaceByColumnId = getWorkspaceByColumnId;
module.exports.getWorkspaceByTicketId = getWorkspaceByTicketId;
