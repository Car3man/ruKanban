const { columnHelper, responseHelper, workspaceHelper } = require('../common/helpers');

/**
 * Change column title business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const columnChangeTitle = async (req, res) => {
  try {
    const { userId } = req;
    const columnId = BigInt(req.query.column_id);
    const { title } = req.body;

    const column = await columnHelper.getColumnById(columnId);

    if (!column) {
      return responseHelper.sendNotFound(req, res);
    }

    const workspaceId = (await workspaceHelper.getWorkspaceByColumnId(columnId)).id;

    const isAllowToUpdate = await workspaceHelper.isUserWorkspaceOwner(userId, workspaceId);
    if (!isAllowToUpdate) {
      return responseHelper.sendForbidden(req, res);
    }

    const titleValidationResult = columnHelper.isColumnTitleValid(title);
    if (!titleValidationResult.isValid) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: titleValidationResult.details,
      });
    }

    await columnHelper.changeColumnTitle(columnId, title);

    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = columnChangeTitle;
