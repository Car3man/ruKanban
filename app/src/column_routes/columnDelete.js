const { workspaceHelper, columnHelper, responseHelper } = require('../common/helpers');

/**
 * Delete column business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const columnDelete = async (req, res) => {
  try {
    const { userId } = req;
    const columnId = BigInt(req.query.column_id);

    const workspaceId = (await workspaceHelper.getWorkspaceByColumnId(columnId)).id;

    const isAllowToDelete = await workspaceHelper.isUserWorkspaceOwner(userId, workspaceId);
    if (!isAllowToDelete) {
      return responseHelper.sendForbidden(req, res);
    }

    await columnHelper.deleteColumn(columnId);

    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = columnDelete;
