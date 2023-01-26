const { ticketHelper, boardHelper, responseHelper } = require('../common/helpers');
const utils = require('../common/utils');

/**
   * Create ticket business logic (safety)
   * @param {import('express').Request} req
   * @param {import('express').Response} res
   */
const ticketGet = async (req, res) => {
  try {
    const { userId } = req;
    const columnId = BigInt(req.query.column_id);
    const startAt = req.query.start_at || 0;
    const limit = req.query.limit || 100;

    const boardId = (await boardHelper.getBoardByColumnId(columnId)).id;

    const isAllowToGet = await boardHelper.isUserBoardParticipant(userId, boardId);
    if (!isAllowToGet) {
      return responseHelper.sendForbidden(req, res);
    }

    const tickets = await ticketHelper.getTicketsByColumnId(columnId, startAt, limit);

    const result = {
      tickets: tickets.map((x) => utils.escapeObjectBigInt(x)),
    };
    return responseHelper.sendOk(req, res, result);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = ticketGet;
