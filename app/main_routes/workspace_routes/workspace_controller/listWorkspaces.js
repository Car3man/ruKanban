const { PrismaClient } = require('@prisma/client');
const responseHelper = require('../../../common/response-helper');
const utils = require('../../../common/utils');

const prisma = new PrismaClient();

/**
 * Sign out business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const listWorkspaces = async (req, res) => {
  try {
    const userId = BigInt(req.userId);
    let { startAt, limit } = req.params;

    startAt = startAt || 0;
    limit = limit || 10;

    const userWorkspaceIds = (await prisma.user_workspace.findMany({
      where: {
        user_id: userId,
      },
      select: {
        workspace_id: true,
      },
      skip: startAt,
      take: limit,
    })).map((x) => x.workspace_id);

    let userWorkspaces = await prisma.workspaces.findMany({
      where: {
        id: { in: userWorkspaceIds },
      },
      select: {
        id: true,
        name: true,
        created_at: true,
      },
    });

    userWorkspaces = userWorkspaces.map((x) => utils.escapeObjectBigInt(x));
    return responseHelper.sendOk(req, res, userWorkspaces);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = listWorkspaces;
