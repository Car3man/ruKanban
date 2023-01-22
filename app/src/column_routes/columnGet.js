const { columnHelper, responseHelper, boardHelper } = require('../common/helpers');
const utils = require('../common/utils');

/**
 * Get columns business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const columnGet = async (req, res) => {
  try {
    const { userId } = req;
    const boardId = BigInt(req.query.board_id);
    const startAt = req.query.start_at || 0;
    const limit = req.query.limit || 100;

    const isAllowToGet = await boardHelper.isUserBoardParticipant(userId, boardId);
    if (!isAllowToGet) {
      return responseHelper.sendForbidden(req, res);
    }

    const columns = await columnHelper.getColumnsByBoardId(boardId, startAt, limit);

    const result = columns.map((x) => utils.escapeObjectBigInt(x));
    return responseHelper.sendOk(req, res, result);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = columnGet;
