const { workspaceHelper, ticketHelper, responseHelper } = require('../common/helpers');

/**
   * Create ticket business logic (safety)
   * @param {import('express').Request} req
   * @param {import('express').Response} res
   */
const ticketDelete = async (req, res) => {
  try {
    const { userId } = req;
    const ticketId = BigInt(req.query.ticket_id);

    const workspaceId = (await workspaceHelper.getWorkspaceByTicketId(ticketId)).id;

    const isAllowToDelete = await workspaceHelper.isUserWorkspaceOwner(userId, workspaceId);
    if (!isAllowToDelete) {
      return responseHelper.sendForbidden(req, res);
    }

    await ticketHelper.deleteTicket(ticketId);

    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = ticketDelete;
