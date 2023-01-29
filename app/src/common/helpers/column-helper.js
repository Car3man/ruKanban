/* eslint-disable no-await-in-loop */

const { PrismaClient } = require('@prisma/client');

const prisma = new PrismaClient();

/**
 * @typedef {Object} ValidationResult
 * @property {Boolean} isValid
 * @property {String|Undefined} details
 * @param {String} title
 * @returns {ValidationResult}
 */
function isColumnTitleValid(title) {
  if (typeof title !== 'string') {
    return { isValid: false, details: 'Title type should be a String.' };
  }

  const trimmedTitle = title.trim();
  if (trimmedTitle.length < 1) {
    return { isValid: false, details: 'Title length should be greater or equal than 1.' };
  }

  if (trimmedTitle.length > 36) {
    return { isValid: false, details: 'Title length should be less or equals than 36' };
  }

  return { isValid: true };
}

/**
 * @async
 * @param {BigInt} boardId
 * @returns {Number}
 */
async function getNextColumnIndex(boardId) {
  const lastIndexedColumn = await prisma.columns.findFirst({
    orderBy: [
      {
        index: 'desc',
      },
    ],
    where: {
      board_id: boardId,
    },
    select: {
      index: true,
    },
  });

  if (!lastIndexedColumn) {
    return 0;
  }

  return lastIndexedColumn.index + 1;
}

/**
 * @async
 * @param {BigInt} boardId
 * @param {String} title
 * @returns {import('@prisma/client').columns}
 */
async function createColumn(boardId, title) {
  const index = await getNextColumnIndex(boardId);

  return prisma.columns.create({
    data: {
      board_id: boardId,
      index,
      title,
      created_at: new Date(),
    },
  });
}

/**
 * @async
 * @param {BigInt} columnId
 * @param {String} title
 */
async function changeColumnTitle(columnId, title) {
  return prisma.columns.update({
    where: { id: columnId },
    data: { title },
  });
}

/**
 * @async
 * @param {BigInt} columnId
 * @param {BigInt} standAfterId
 */
async function moveColumn(columnId, standAfterId) {
  return prisma.$transaction(async (tx) => {
    const column = await tx.columns.findFirst({
      where: { id: columnId },
      select: { id: true },
    });

    const columns = await tx.columns.findMany({
      where: {
        board_id: column.board_id,
      },
      select: { id: true, index: true },
      orderBy: [
        {
          index: 'asc',
        },
      ],
    });

    let standAfterIndex = -1;
    let indexOffset = standAfterId ? 0 : 1;
    for (let i = 0; i < columns.length; i += 1) {
      const { id } = columns[i];

      if (id !== columnId) {
        await tx.columns.update({
          where: { id },
          data: { index: i + indexOffset },
        });

        if (id === standAfterId) {
          standAfterIndex = i + indexOffset;
          indexOffset += 1;
        }
      }
    }

    await tx.columns.update({
      where: { id: columnId },
      data: {
        index: standAfterIndex + 1,
      },
    });

    const columnsToSort = await tx.columns.findMany({
      where: {
        board_id: column.board_id,
      },
      select: { id: true, index: true },
      orderBy: [
        {
          index: 'asc',
        },
      ],
    });

    for (let i = 0; i < columnsToSort.length; i += 1) {
      const { id } = columnsToSort[i];
      await tx.columns.update({
        where: { id },
        data: { index: i },
      });
    }
  });
}

/**
 * @async
 * @param {BigInt} columnId
 */
async function deleteColumn(columnId) {
  return prisma.columns.delete({
    where: {
      id: columnId,
    },
  });
}

/**
 * @async
 * @param {BigInt} boardId
 * @param {Number} skip
 * @param {Number} take
 * @returns {import('@prisma/client').columns}
 */
async function getColumnsByBoardId(boardId, skip, take) {
  return prisma.columns.findMany({
    where: {
      board_id: boardId,
    },
    select: {
      id: true,
      board_id: true,
      index: true,
      title: true,
      created_at: true,
    },
    orderBy: [
      { index: 'asc' },
    ],
    skip,
    take,
  });
}

/**
 * @async
 * @param {BigInt} columnId
 * @returns {import('@prisma/client').columns}
 */
async function getColumnById(columnId) {
  return prisma.columns.findFirst({
    where: {
      id: columnId,
    },
    select: {
      id: true,
      board_id: true,
      index: true,
      title: true,
      created_at: true,
    },
  });
}

/**
 * @async
 * @param {BigInt} ticketId
 * @returns {import('@prisma/client').columns}
 */
async function getColumnByTicketId(ticketId) {
  const columnId = (await prisma.tickets.findFirst({
    where: {
      id: ticketId,
    },
    select: {
      column_id: true,
    },
  })).column_id;

  return prisma.columns.findFirst({
    where: {
      id: columnId,
    },
    select: {
      id: true,
      board_id: true,
      index: true,
      title: true,
      created_at: true,
    },
  });
}

module.exports.isColumnTitleValid = isColumnTitleValid;
module.exports.getNextColumnIndex = getNextColumnIndex;
module.exports.createColumn = createColumn;
module.exports.changeColumnTitle = changeColumnTitle;
module.exports.moveColumn = moveColumn;
module.exports.deleteColumn = deleteColumn;
module.exports.getColumnsByBoardId = getColumnsByBoardId;
module.exports.getColumnById = getColumnById;
module.exports.getColumnByTicketId = getColumnByTicketId;
