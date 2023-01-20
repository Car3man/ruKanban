const { workspaceHelper, responseHelper, boardHelper } = require('../common/helpers');
const utils = require('../common/utils');

/**
 * Get board by id business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const boardGetById = async (req, res) => {
  try {
    const { userId } = req;
    const boardId = BigInt(req.query.board_id);

    const board = await boardHelper.getBoardById(boardId);

    if (!board) {
      return responseHelper.sendNotFound(req, res);
    }

    const workspaceId = (await workspaceHelper.getWorkspaceByBoardId(boardId)).id;

    const isAllowToGet = await workspaceHelper.isUserWorkspaceParticipant(userId, workspaceId);
    if (!isAllowToGet) {
      return responseHelper.sendForbidden(req, res);
    }

    const result = utils.escapeObjectBigInt(board);
    return responseHelper.sendOk(req, res, result);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = boardGetById;
