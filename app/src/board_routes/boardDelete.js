const { workspaceHelper, responseHelper, boardHelper } = require('../common/helpers');

/**
 * Delete board business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const boardDelete = async (req, res) => {
  try {
    const { userId } = req;
    const boardId = BigInt(req.query.board_id);

    const board = await boardHelper.getBoardById(boardId);

    if (!board) {
      return responseHelper.sendNotFound(req, res);
    }

    const workspaceId = (await workspaceHelper.getWorkspaceByBoardId(boardId)).id;

    const isAllowToDelete = await workspaceHelper.isUserWorkspaceOwner(userId, workspaceId);
    if (!isAllowToDelete) {
      return responseHelper.sendForbidden(req, res);
    }

    await boardHelper.deleteBoard(boardId);

    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = boardDelete;
