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
    const newColumnName = req.body.name;

    const column = await columnHelper.getColumnById(columnId);

    if (!column) {
      return responseHelper.sendNotFound(req, res);
    }

    const workspaceId = (await workspaceHelper.getWorkspaceByColumnId(columnId)).id;

    const isAllowToUpdate = await workspaceHelper.isUserWorkspaceOwner(userId, workspaceId);
    if (!isAllowToUpdate) {
      return responseHelper.sendForbidden(req, res);
    }

    if (newColumnName) {
      const nameValidationResult = columnHelper.isColumnNameValid(newColumnName);
      if (!nameValidationResult.isValid) {
        return responseHelper.sendBadRequest(req, res, {
          error_msg: nameValidationResult.details,
        });
      }
    }

    await columnHelper.updateColumn(columnId, newColumnIndex, newColumnName);

    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = columnUpdate;
