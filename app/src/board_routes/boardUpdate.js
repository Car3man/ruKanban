const { PrismaClient } = require('@prisma/client');

const { workspaceHelper, boardHelper, responseHelper } = require('../common/helpers');
const UpdateBoardError = require('../common/errors/UpdateBoardError');

const prisma = new PrismaClient();

/**
 * Update board business logic (safety)
 * @param {import('express').Request} req
 * @param {import('express').Response} res
 */
const boardUpdate = async (req, res) => {
  try {
    const { userId } = req;
    const boardId = BigInt(req.query.board_id);
    const newBoardName = req.body.name;
    const newBoardDescription = req.body.description;
    const usersToAdd = req.body.usersToAdd ? req.body.usersToAdd.map((x) => BigInt(x)) : undefined;
    const usersToDelete = req.body.usersToDelete ? req.body.usersToDelete.map((x) => BigInt(x)) : undefined;

    const board = await boardHelper.getBoardById(boardId);

    if (!board) {
      return responseHelper.sendNotFound(req, res);
    }

    const workspaceId = (await workspaceHelper.getWorkspaceByBoardId(boardId)).id;

    const isAllowToUpdate = await workspaceHelper.isUserWorkspaceOwner(userId, workspaceId);
    if (!isAllowToUpdate) {
      return responseHelper.sendForbidden(req, res);
    }

    if (newBoardName) {
      const nameValidationResult = boardHelper.isBoardNameValid(newBoardName);
      if (!nameValidationResult.isValid) {
        return responseHelper.sendBadRequest(req, res, {
          error_msg: nameValidationResult.details,
        });
      }
    }

    if (newBoardDescription) {
      const descriptionValidationResult = boardHelper.isBoardDescriptionValid(newBoardDescription);
      if (!descriptionValidationResult.isValid) {
        return responseHelper.sendBadRequest(req, res, {
          error_msg: descriptionValidationResult.details,
        });
      }
    }

    const boardOwnerId = (await prisma.user_workspace.findFirst({
      where: {
        workspace_id: workspaceId,
        workspace_roles: {
          is: {
            name: 'owner',
          },
        },
      },
    })).user_id;

    if (usersToDelete && usersToDelete.includes(boardOwnerId)) {
      return responseHelper.sendBadRequest(req, res, {
        error_msg: 'Impossible delete yourself from board.',
      });
    }

    try {
      await boardHelper.updateBoard(boardId, newBoardName, newBoardDescription, usersToAdd, usersToDelete);
    } catch (err) {
      if (err instanceof UpdateBoardError) {
        return responseHelper.sendBadRequest(req, res, {
          error_msg: err.message,
        });
      }
      throw err;
    }

    return responseHelper.sendOk(req, res);
  } catch (err) {
    console.log(err);
    return responseHelper.sendInternalServerError(req, res);
  }
};

module.exports = boardUpdate;
