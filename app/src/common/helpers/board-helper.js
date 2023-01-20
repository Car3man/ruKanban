/* eslint-disable no-await-in-loop */
/* eslint-disable no-restricted-syntax */

const { PrismaClient } = require('@prisma/client');

const workspaceHelper = require('./workspace-helper');
const columnHelper = require('./column-helper');
const UpdateBoardError = require('../errors/UpdateBoardError');

const prisma = new PrismaClient();

/**
 * @typedef {Object} ValidationResult
 * @property {Boolean} isValid
 * @property {String|Undefined} details
 * @param {String} name
 * @returns {ValidationResult}
 */
function isBoardNameValid(name) {
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
 * @typedef {Object} ValidationResult
 * @property {Boolean} isValid
 * @property {String|Undefined} details
 * @param {String} description
 * @returns {ValidationResult}
 */
function isBoardDescriptionValid(description) {
  if (typeof description !== 'string') {
    return { isValid: false, details: 'Description type should be a String.' };
  }

  const trimmedDescription = description.trim();

  if (trimmedDescription.length > 36) {
    return { isValid: false, details: 'Description length should be less or equals than 36' };
  }

  return { isValid: true };
}

/**
 * @param {BigInt} userId
 * @param {BigInt} boardId
 * @returns {Boolean}
 */
async function isUserBoardParticipant(userId, boardId) {
  const workspaceId = (await workspaceHelper.getWorkspaceByBoardId(boardId)).id;
  const isUserWorkspaceParticipan = await workspaceHelper.isUserWorkspaceParticipant(userId, workspaceId);

  if (!isUserWorkspaceParticipan) {
    return false;
  }

  return await prisma.user_board.count({
    where: {
      user_id: userId,
      board_id: boardId,
    },
  }) > 0;
}

/**
 * @async
 * @param {BigInt} workspaceId
 * @param {String} name
 * @param {String} description
 * @param {Array<BigInt>} boardUserIds
 * @returns {import('@prisma/client').boards}
 */
async function createBoard(workspaceId, name, description, boardUserIds) {
  return prisma.$transaction(async (tx) => {
    const board = await tx.boards.create({
      data: {
        workspace_id: workspaceId,
        name,
        description,
        created_at: new Date(),
      },
    });

    for (const boardUserId of boardUserIds) {
      await tx.user_board.create({
        data: {
          user_id: boardUserId,
          board_id: board.id,
        },
      });
    }

    return board;
  });
}

/**
 * @async
 * @param {BigInt} id
 * @param {String|Undefined} newName
 * @param {String|Undefined} newDescription
 * @param {Array<BigInt>|Undefined} usersToAdd
 * @param {Array<BigInt>|Undefined} usersToDelete
 */
async function updateBoard(id, newName, newDescription, usersToAdd, usersToDelete) {
  return prisma.$transaction(async (tx) => {
    if (newName) {
      await tx.boards.update({
        where: { id },
        data: { name: newName },
      });
    }

    if (newDescription) {
      await tx.boards.update({
        where: { id },
        data: { description: newDescription },
      });
    }

    if (usersToAdd || usersToDelete) {
      const boardUserIds = (await tx.user_board.findMany({
        where: { board_id: id },
        select: { user_id: true },
      })).map((x) => x.user_id);

      if (usersToAdd && usersToAdd.length > 0) {
        for (const userToAdd of usersToAdd) {
          if (boardUserIds.includes(userToAdd)) {
            throw new UpdateBoardError(`User with user_id = '${userToAdd}' already added.`);
          }
        }

        const userBoardRecordsToInsert = [];
        for (const userToAdd of usersToAdd) {
          userBoardRecordsToInsert.push({
            user_id: userToAdd,
            board_id: id,
          });
        }

        await prisma.user_board.createMany({
          data: userBoardRecordsToInsert,
        });
      }

      if (usersToDelete && usersToDelete.length > 0) {
        for (const userToDelete of usersToDelete) {
          if (!boardUserIds.includes(userToDelete)) {
            throw new UpdateBoardError(`User with user_id = '${userToDelete}' there is no in the list of users.`);
          }
        }

        await prisma.user_board.deleteMany({
          where: {
            user_id: { in: usersToDelete },
            board_id: id,
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
async function deleteBoard(id) {
  return prisma.$transaction(async (tx) => {
    await tx.user_board.deleteMany({
      where: {
        board_id: id,
      },
    });

    await tx.boards.delete({
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
 * @returns {import('@prisma/client').boards}
 */
async function getBoardsByUserId(userId, skip, take) {
  const boardIds = (await prisma.user_board.findMany({
    where: {
      user_id: userId,
    },
    select: {
      board_id: true,
    },
    skip,
    take,
  })).map((x) => x.board_id);

  return prisma.boards.findMany({
    where: {
      id: { in: boardIds },
    },
    select: {
      id: true,
      workspace_id: true,
      name: true,
      description: true,
      created_at: true,
    },
  });
}

/**
 * @async
 * @param {BigInt} workspaceId
 * @param {Number} skip
 * @param {Number} take
 * @returns {import('@prisma/client').boards}
 */
async function getBoardsByWorkspaceId(workspaceId, skip, take) {
  return prisma.boards.findMany({
    where: {
      workspace_id: workspaceId,
    },
    select: {
      id: true,
      workspace_id: true,
      name: true,
      description: true,
      created_at: true,
    },
    skip,
    take,
  });
}

/**
 * @async
 * @param {BigInt} boardId
 * @returns {import('@prisma/client').boards}
 */
async function getBoardById(boardId) {
  return prisma.boards.findFirst({
    where: {
      id: boardId,
    },
    select: {
      id: true,
      workspace_id: true,
      name: true,
      description: true,
      created_at: true,
    },
  });
}

/**
 * @async
 * @param {BigInt} columnId
 * @returns {import('@prisma/client').boards}
 */
async function getBoardByColumnId(columnId) {
  const boardId = (await prisma.columns.findFirst({
    where: {
      id: columnId,
    },
    select: {
      board_id: true,
    },
  })).board_id;

  return prisma.boards.findFirst({
    where: {
      id: boardId,
    },
    select: {
      id: true,
      workspace_id: true,
      name: true,
      description: true,
      created_at: true,
    },
  });
}

/**
 * @async
 * @param {BigInt} ticketId
 * @returns {import('@prisma/client').boards}
 */
async function getBoardByTicketId(ticketId) {
  const boardId = (await columnHelper.getColumnByTicketId(ticketId)).board_id;

  return prisma.boards.findFirst({
    where: {
      id: boardId,
    },
    select: {
      id: true,
      workspace_id: true,
      name: true,
      description: true,
      created_at: true,
    },
  });
}

module.exports.isBoardNameValid = isBoardNameValid;
module.exports.isBoardDescriptionValid = isBoardDescriptionValid;
module.exports.isUserBoardParticipant = isUserBoardParticipant;
module.exports.createBoard = createBoard;
module.exports.updateBoard = updateBoard;
module.exports.deleteBoard = deleteBoard;
module.exports.getBoardsByUserId = getBoardsByUserId;
module.exports.getBoardsByWorkspaceId = getBoardsByWorkspaceId;
module.exports.getBoardById = getBoardById;
module.exports.getBoardByColumnId = getBoardByColumnId;
module.exports.getBoardByTicketId = getBoardByTicketId;
