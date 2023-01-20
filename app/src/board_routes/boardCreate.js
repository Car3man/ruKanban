const { workspaceHelper, boardHelper, responseHelper } = require('../common/helpers');
const utils = require('../common/utils');

/**
 * Create board business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const boardCreate = async (req, res) => {
  try {
    const { userId } = req;
    const workspaceId = BigInt(req.query.workspace_id);
    const { name, description } = req.body;

    const nameValidationResult = boardHelper.isBoardNameValid(name);
    if (!nameValidationResult.isValid) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: nameValidationResult.details,
      });
    }

    const descriptionValidationResult = boardHelper.isBoardDescriptionValid(description);
    if (!descriptionValidationResult.isValid) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: descriptionValidationResult.details,
      });
    }

    const isAllowToCreate = await workspaceHelper.isUserWorkspaceOwner(userId, workspaceId);
    if (!isAllowToCreate) {
      return responseHelper.sendForbidden(req, res);
    }

    const board = await boardHelper.createBoard(workspaceId, name, description, [userId]);

    const result = utils.escapeObjectBigInt(board);
    return responseHelper.sendOk(req, res, result);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = boardCreate;
