const { workspaceHelper, ticketHelper, responseHelper } = require('../common/helpers');
const utils = require('../common/utils');

/**
 * Create ticket business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const ticketCreate = async (req, res) => {
  try {
    const { userId } = req;
    const columnId = BigInt(req.query.column_id);
    const { title, description } = req.body;

    const titleValidationResult = ticketHelper.isTicketTitleValid(title);
    if (!titleValidationResult.isValid) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: titleValidationResult.details,
      });
    }

    const descriptionValidationResult = ticketHelper.isTicketDescriptionValid(description);
    if (!descriptionValidationResult.isValid) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: descriptionValidationResult.details,
      });
    }

    const workspaceId = (await workspaceHelper.getWorkspaceByColumnId(columnId)).id;

    const isAllowToCreate = await workspaceHelper.isUserWorkspaceParticipant(userId, workspaceId);
    if (!isAllowToCreate) {
      return responseHelper.sendForbidden(req, res);
    }

    const ticket = await ticketHelper.createTicket(columnId, title, description);

    const result = {
      ticket: utils.escapeObjectBigInt(ticket),
    };
    return responseHelper.sendOk(req, res, result);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = ticketCreate;
