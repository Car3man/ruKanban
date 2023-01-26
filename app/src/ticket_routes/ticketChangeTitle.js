const { workspaceHelper, ticketHelper, responseHelper } = require('../common/helpers');

/**
 * Change ticket name business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const ticketChangeTitle = async (req, res) => {
  try {
    const { userId } = req;
    const ticketId = BigInt(req.query.ticket_id);
    const { title } = req.body;

    const ticket = await ticketHelper.getTicketById(ticketId);

    if (!ticket) {
      return responseHelper.sendNotFound(req, res);
    }

    const workspaceId = (await workspaceHelper.getWorkspaceByTicketId(ticketId)).id;

    const isAllowToUpdate = await workspaceHelper.isUserWorkspaceParticipant(userId, workspaceId);
    if (!isAllowToUpdate) {
      return responseHelper.sendForbidden(req, res);
    }

    const titleValidationResult = ticketHelper.isTicketTitleValid(title);
    if (!titleValidationResult.isValid) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: titleValidationResult.details,
      });
    }

    await ticketHelper.changeTicketTitle(ticketId, title);

    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = ticketChangeTitle;
