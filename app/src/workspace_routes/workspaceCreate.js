const { workspaceHelper, responseHelper } = require('../common/helpers');
const utils = require('../common/utils');

/**
 * Create workspace business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const workspaceCreate = async (req, res) => {
  try {
    const { userId } = req;
    const { name } = req.body;

    const nameValidationResult = workspaceHelper.isWorkspaceNameValid(name);
    if (!nameValidationResult.isValid) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: nameValidationResult.details,
      });
    }

    const workspace = await workspaceHelper.createWorkspace(name, [{
      userId,
      roleName: 'owner',
    }]);

    const result = utils.escapeObjectBigInt(workspace);
    return responseHelper.sendOk(req, res, result);
  } catch (err) {
    console.error(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = workspaceCreate;
