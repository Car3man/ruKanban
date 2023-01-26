const { workspaceHelper, columnHelper, responseHelper } = require('../common/helpers');
const utils = require('../common/utils');

/**
 * Create column business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const columnCreate = async (req, res) => {
  try {
    const { userId } = req;
    const boardId = BigInt(req.query.board_id);
    const { name } = req.body;

    const nameValidationResult = columnHelper.isColumnNameValid(name);
    if (!nameValidationResult.isValid) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: nameValidationResult.details,
      });
    }

    const workspaceId = (await workspaceHelper.getWorkspaceByBoardId(boardId)).id;

    const isAllowToCreate = await workspaceHelper.isUserWorkspaceParticipant(userId, workspaceId);
    if (!isAllowToCreate) {
      return responseHelper.sendForbidden(req, res);
    }

    const column = await columnHelper.createColumn(boardId, name);

    const result = {
      column: utils.escapeObjectBigInt(column),
    };
    return responseHelper.sendOk(req, res, result);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = columnCreate;
