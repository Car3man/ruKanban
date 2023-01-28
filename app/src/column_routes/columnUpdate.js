const { columnHelper, responseHelper, workspaceHelper } = require('../common/helpers');

/**
 * Update column business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const columnUpdate = async (req, res) => {
  try {
    const { userId } = req;
    const columnId = BigInt(req.query.column_id);
    const newColumnIndex = req.body.index;
    const newColumnTitle = req.body.title;

    const column = await columnHelper.getColumnById(columnId);

    if (!column) {
      return responseHelper.sendNotFound(req, res);
    }

    const workspaceId = (await workspaceHelper.getWorkspaceByColumnId(columnId)).id;

    const isAllowToUpdate = await workspaceHelper.isUserWorkspaceOwner(userId, workspaceId);
    if (!isAllowToUpdate) {
      return responseHelper.sendForbidden(req, res);
    }

    if (newColumnTitle) {
      const titleValidationResult = columnHelper.isColumnTitleValid(newColumnTitle);
      if (!titleValidationResult.isValid) {
        return responseHelper.sendBadRequest(req, res, {
          error_msg: titleValidationResult.details,
        });
      }
    }

    await columnHelper.updateColumn(columnId, newColumnIndex, newColumnTitle);

    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = columnUpdate;
