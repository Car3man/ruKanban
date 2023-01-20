const { workspaceHelper, ticketHelper, responseHelper } = require('../common/helpers');
const utils = require('../common/utils');

/**
   * Create ticket business logic (safety)
   * @param {import('express').Request} req
   * @param {import('express').Response} res
   */
const ticketGetById = async (req, res) => {
  try {
    const { userId } = req;
    const ticketId = BigInt(req.query.ticket_id);

    const ticket = await ticketHelper.getTicketById(ticketId);

    if (!ticket) {
      return responseHelper.sendNotFound(req, res);
    }

    const workspaceId = (await workspaceHelper.getWorkspaceByTicketId(ticketId)).id;

    const isAllowToGet = await workspaceHelper.isUserWorkspaceParticipant(userId, workspaceId);
    if (!isAllowToGet) {
      return responseHelper.sendForbidden(req, res);
    }

    const result = utils.escapeObjectBigInt(ticket);
    return responseHelper.sendOk(req, res, result);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = ticketGetById;
