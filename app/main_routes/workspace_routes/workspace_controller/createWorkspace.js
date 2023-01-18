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
const createWorkspace = async (req, res) => {
  try {
    const { userId, login } = req;
    const { name } = req.body;

    const isAllow = await workspaceHelper.isUserCanCreateWorkspaceAsync(userId);
    if (!isAllow) {
      return responseHelper.sendForbidden(req, res);
    }

    let createResult = await prisma.workspaces.create({
      data: {
        name,
        created_at: new Date(),
        user_workspace: {
          create: [
            {
              users: {
                connect: {
                  login,
                },
              },
              workspace_roles: {
                connect: {
                  name: 'owner',
                },
              },
            },
          ],
        },
      },
    });

    createResult = utils.escapeBigInt(createResult);
    return responseHelper.sendOk(req, res, createResult);
  } catch (err) {
    console.error(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = createWorkspace;
