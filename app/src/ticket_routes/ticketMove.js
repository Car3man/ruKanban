const { workspaceHelper, ticketHelper, responseHelper } = require('../common/helpers');

/**
 * Move ticket business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const ticketMove = async (req, res) => {
  try {
    const { userId } = req;
    const ticketId = BigInt(req.query.ticket_id);
    const columnId = BigInt(req.body.column_id);
    const standAfterId = BigInt(req.body.stand_after_id);

    const ticket = await ticketHelper.getTicketById(ticketId);

    if (!ticket) {
      return responseHelper.sendNotFound(req, res);
    }

    const workspaceId = (await workspaceHelper.getWorkspaceByTicketId(ticketId)).id;

    const isAllowToUpdate = await workspaceHelper.isUserWorkspaceParticipant(userId, workspaceId);
    if (!isAllowToUpdate) {
      return responseHelper.sendForbidden(req, res);
    }

    await ticketHelper.moveTicket(ticket.id, columnId, standAfterId);

    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = ticketMove;
