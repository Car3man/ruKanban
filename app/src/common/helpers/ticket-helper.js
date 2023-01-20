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
 * @param {Number} newIndex
 * @param {String} newTitle
 * @param {String} newDescription
 */
async function updateTicket(ticketId, newIndex, newTitle, newDescription) {
  return prisma.$transaction(async (tx) => {
    if (newIndex) {
      await tx.tickets.update({
        where: { id: ticketId },
        data: { index: newIndex },
      });
    }

    if (newTitle) {
      await tx.tickets.update({
        where: { id: ticketId },
        data: { title: newTitle },
      });
    }

    if (newDescription) {
      await tx.tickets.update({
        where: { id: ticketId },
        data: { description: newDescription },
      });
    }
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
module.exports.updateTicket = updateTicket;
module.exports.deleteTicket = deleteTicket;
module.exports.getTicketsByColumnId = getTicketsByColumnId;
module.exports.getTicketById = getTicketById;
