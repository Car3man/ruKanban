const { PrismaClient } = require('@prisma/client');
const workspaceHelper = require('../../../common/workspace-helper');
const responseHelper = require('../../../common/response-helper');
const utils = require('../../../common/utils');

const prisma = new PrismaClient();

/**
 * Sign out business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const getWorkspace = async (req, res) => {
  try {
    const userId = BigInt(req.userId);
    const workspaceId = BigInt(req.params.id);

    let getResult = await prisma.workspaces.findFirst({
      where: {
        id: workspaceId,
      },
      select: {
        id: true,
        name: true,
        created_at: true,
      },
    });

    if (!getResult) {
      return responseHelper.sendNotFound(req, res);
    }

    const isAllow = await workspaceHelper.isUserCanGetWorkspaceAsync(userId, workspaceId);
    if (!isAllow) {
      return responseHelper.sendForbidden(req, res);
    }

    getResult = utils.escapeObjectBigInt(getResult);
    return responseHelper.sendOk(req, res, getResult);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = getWorkspace;
