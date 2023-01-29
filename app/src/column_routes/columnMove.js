const { columnHelper, responseHelper, workspaceHelper } = require('../common/helpers');

/**
 * Column move business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const columnMove = async (req, res) => {
  try {
    const { userId } = req;
    const columnId = BigInt(req.query.column_id);
    const standAfterId = BigInt(req.body.stand_after_id);

    const column = await columnHelper.getColumnById(columnId);

    if (!column) {
      return responseHelper.sendNotFound(req, res);
    }

    const workspaceId = (await workspaceHelper.getWorkspaceByColumnId(columnId)).id;

    const isAllowToUpdate = await workspaceHelper.isUserWorkspaceParticipant(userId, workspaceId);
    if (!isAllowToUpdate) {
      return responseHelper.sendForbidden(req, res);
    }

    await columnHelper.moveColumn(columnId, standAfterId);

    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = columnMove;
