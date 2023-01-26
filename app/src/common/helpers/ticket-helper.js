/* eslint-disable no-await-in-loop */
/* eslint-disable no-restricted-syntax */

const { PrismaClient } = require('@prisma/client');

const prisma = new PrismaClient();

/**
 * @typedef {Object} ValidationResult
 * @property {Boolean} isValid
 * @property {String|Undefined} details
 * @param {String} title
 * @returns {ValidationResult}
 */
function isTicketTitleValid(title) {
  if (typeof title !== 'string') {
    return { isValid: false, details: 'Title type should be a String.' };
  }

  const trimmedTitle = title.trim();
  if (trimmedTitle.length < 1) {
    return { isValid: false, details: 'Title length should be greater than 1.' };
  }

  if (trimmedTitle.length > 128) {
    return { isValid: false, details: 'Title length should be less or equals than 128' };
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
function isTicketDescriptionValid(description) {
  if (typeof description !== 'string') {
    return { isValid: false, details: 'Description type should be a String.' };
  }

  const trimmedDescription = description.trim();

  if (trimmedDescription.length > 2048) {
    return { isValid: false, details: 'Description length should be less or equals than 128' };
  }

  return { isValid: true };
}

/**
 * @async
 * @param {BigInt} columnId
 * @returns {Number}
 */
async function getNextTicketIndex(columnId) {
  const lastIndexedTicket = await prisma.tickets.findFirst({
    orderBy: [
      {
        index: 'desc',
      },
    ],
    where: {
      column_id: columnId,
    },
    select: {
      index: true,
    },
  });

  if (!lastIndexedTicket) {
    return 0;
  }

  return lastIndexedTicket.index + 1;
}

/**
 * @async
 * @param {BigInt} columnId
 * @param {String} title
 * @param {String} description
 * @returns {import('@prisma/client').columns}
 */
async function createTicket(columnId, title, description) {
  const index = await getNextTicketIndex(columnId);

  return prisma.tickets.create({
    data: {
      column_id: columnId,
      index,
      title,
      description,
      created_at: new Date(),
    },
  });
}

/**
 * @async
 * @param {BigInt} ticketId
 * @param {String} title
 */
async function changeTicketTitle(ticketId, title) {
  return prisma.tickets.update({
    where: { id: ticketId },
    data: { title },
  });
}

/**
 * @async
 * @param {BigInt} ticketId
 * @param {String} description
 */
async function changeTicketDescription(ticketId, description) {
  return prisma.tickets.update({
    where: { id: ticketId },
    data: { description },
  });
}

/**
 * @async
 * @param {BigInt} ticketId
 * @param {BigInt} columnId
 * @param {Number} index
 */
async function moveTicket(ticketId, columnId, index) {
  return prisma.$transaction(async (tx) => {
    const ticketsToUpdate = await tx.tickets.findMany({
      where: {
        column_id: columnId,
        index: {
          gte: index,
        },
      },
      select: { id: true },
      orderBy: [
        {
          index: 'desc',
        },
      ],
    });

    for (const ticketToUpdate of ticketsToUpdate) {
      await tx.tickets.update({
        where: {
          id: ticketToUpdate.id,
        },
        data: {
          index: {
            increment: 1,
          },
        },
      });
    }

    await tx.tickets.update({
      where: { id: ticketId },
      data: {
        column_id: columnId,
        index,
      },
    });
  });
}

/**
 * @async
 * @param {BigInt} ticketId
 */
async function deleteTicket(ticketId) {
  return prisma.tickets.delete({
    where: {
      id: ticketId,
    },
  });
}

/**
 * @async
 * @param {BigInt} columnId
 * @param {Number} skip
 * @param {Number} take
 * @returns {import('@prisma/client').tickets}
 */
async function getTicketsByColumnId(columnId, skip, take) {
  return prisma.tickets.findMany({
    where: {
      column_id: columnId,
    },
    select: {
      id: true,
      column_id: true,
      index: true,
      title: true,
      description: true,
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
 * @param {BigInt} ticketId
 * @returns {import('@prisma/client').tickets}
 */
async function getTicketById(ticketId) {
  return prisma.tickets.findFirst({
    where: {
      id: ticketId,
    },
    select: {
      id: true,
      column_id: true,
      index: true,
      title: true,
      description: true,
      created_at: true,
    },
  });
}

module.exports.isTicketTitleValid = isTicketTitleValid;
module.exports.isTicketDescriptionValid = isTicketDescriptionValid;
module.exports.getNextTicketIndex = getNextTicketIndex;
module.exports.createTicket = createTicket;
module.exports.changeTicketTitle = changeTicketTitle;
module.exports.changeTicketDescription = changeTicketDescription;
module.exports.moveTicket = moveTicket;
module.exports.deleteTicket = deleteTicket;
module.exports.getTicketsByColumnId = getTicketsByColumnId;
module.exports.getTicketById = getTicketById;
