const { responseHelper, workspaceHelper, boardHelper } = require('../common/helpers');
const utils = require('../common/utils');

/**
 * Get boards business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const boardGet = async (req, res) => {
  try {
    const { userId } = req;
    const workspaceId = BigInt(req.query.workspace_id);
    const startAt = req.query.start_at || 0;
    const limit = req.query.limit || 10;

    const isAllowToGet = await workspaceHelper.isUserWorkspaceParticipant(userId, workspaceId);
    if (!isAllowToGet) {
      return responseHelper.sendForbidden(req, res);
    }

    const boards = await boardHelper.getBoardsByWorkspaceId(workspaceId, startAt, limit);

    const result = boards.map((x) => utils.escapeObjectBigInt(x));
    return responseHelper.sendOk(req, res, result);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = boardGet;
