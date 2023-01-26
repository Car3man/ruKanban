const { columnHelper, responseHelper, workspaceHelper } = require('../common/helpers');
const utils = require('../common/utils');

/**
 * Get column by id business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const columnGetById = async (req, res) => {
  try {
    const { userId } = req;
    const columnId = BigInt(req.query.column_id);

    const column = await columnHelper.getColumnById(columnId);

    if (!column) {
      return responseHelper.sendNotFound(req, res);
    }

    const workspaceId = (await workspaceHelper.getWorkspaceByColumnId(columnId)).id;

    const isAllowToGet = await workspaceHelper.isUserWorkspaceParticipant(userId, workspaceId);
    if (!isAllowToGet) {
      return responseHelper.sendForbidden(req, res);
    }

    const result = {
      column: utils.escapeObjectBigInt(column),
    };
    return responseHelper.sendOk(req, res, result);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = columnGetById;
