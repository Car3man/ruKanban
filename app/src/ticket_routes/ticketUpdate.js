const { workspaceHelper, ticketHelper, responseHelper } = require('../common/helpers');

/**
   * Create ticket business logic (safety)
   * @param {import('express').Request} req
   * @param {import('express').Response} res
   */
const ticketUpdate = async (req, res) => {
  try {
    const { userId } = req;
    const ticketId = BigInt(req.query.ticket_id);
    const newTicketColumnId = req.body.column_id ? BigInt(req.body.column_id) : undefined;
    const newTicketIndex = req.body.index ? Number(req.body.index) : undefined;
    const newTicketTitle = req.body.title;
    const newTicketDescription = req.body.description;

    const ticket = await ticketHelper.getTicketById(ticketId);

    if (!ticket) {
      return responseHelper.sendNotFound(req, res);
    }

    const workspaceId = (await workspaceHelper.getWorkspaceByTicketId(ticketId)).id;

    const isAllowToUpdate = await workspaceHelper.isUserWorkspaceOwner(userId, workspaceId);
    if (!isAllowToUpdate) {
      return responseHelper.sendForbidden(req, res);
    }

    if (newTicketTitle) {
      const titleValidationResult = ticketHelper.isTicketTitleValid(newTicketTitle);
      if (!titleValidationResult.isValid) {
        return responseHelper.sendBadRequest(req, res, {
          error_msg: titleValidationResult.details,
        });
      }
    }

    if (newTicketDescription) {
      const descriptionValidationResult = ticketHelper.isTicketDescriptionValid(newTicketDescription);
      if (!descriptionValidationResult.isValid) {
        return responseHelper.sendBadRequest(req, res, {
          error_msg: descriptionValidationResult.details,
        });
      }
    }

    await ticketHelper.updateTicket(ticketId, newTicketColumnId, newTicketIndex, newTicketTitle, newTicketDescription);

    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = ticketUpdate;
