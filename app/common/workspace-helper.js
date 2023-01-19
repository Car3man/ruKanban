const { PrismaClient } = require('@prisma/client');

const prisma = new PrismaClient();

/**
 * @typedef {Object} ValidationResult
 * @property {Boolean} isValid
 * @property {String|Undefined} details
 * @param {String} name
 * @returns {ValidationResult}
 */
const isWorkspaceNameValid = (name) => {
  if (typeof name !== 'string') {
    return { isValid: false, details: 'Name type should be a String.' };
  }

  const trimmedName = name.trim();
  if (trimmedName.length < 5) {
    return { isValid: false, details: 'Name length should be greater than 5.' };
  }

  if (trimmedName.length > 36) {
    return { isValid: false, details: 'Name length should be less or equals than 36' };
  }

  return { isValid: true };
};

/**
 * @param {BigInt} userId
 * @param {BigInt} workspaceId
 */
const getUserWorkspaceRoleName = async (userId, workspaceId) => {
  const userWorkspace = await prisma.user_workspace.findFirst({
    where: {
      user_id: userId,
      workspace_id: workspaceId,
    },
    select: {
      workspace_role_id: true,
    },
  });

  if (!userWorkspace) {
    return null;
  }

  const userWorkspaceRoleId = userWorkspace.id;

  const userWorkspaceRoleName = (await prisma.workspace_roles.findFirstOrThrow({
    where: {
      id: userWorkspaceRoleId,
    },
    select: { name: true },
  })).name;

  return userWorkspaceRoleName;
};

/**
 * @param {BigInt} userId
 * @param {BigInt} workspaceId
 * @returns {Boolean}
 */
const isUserWorkspaceRoleOwner = async (userId, workspaceId) => {
  const userWorkspaceRoleName = await getUserWorkspaceRoleName(userId, workspaceId);
  return userWorkspaceRoleName === 'owner';
};

/**
 * @param {BigInt} userId
 * @param {BigInt} workspaceId
 * @returns {Boolean}
 */
const isUserWorkspaceRoleOwnerOrUser = async (userId, workspaceId) => {
  const userWorkspaceRoleName = await getUserWorkspaceRoleName(userId, workspaceId);
  return userWorkspaceRoleName === 'owner' || userWorkspaceRoleName === 'user';
};

/**
 * @property {BigInt} userId
 * @property {BigInt} workspaceId
 */
const isUserCanGetWorkspaceAsync = async (userId, workspaceId) => isUserWorkspaceRoleOwnerOrUser(userId, workspaceId);

const isUserCanCreateWorkspaceAsync = () => true;

/**
 * @property {BigInt} userId
 * @property {BigInt} workspaceId
 */
const isUserCanUpdateWorkspaceAsync = async (userId, workspaceId) => isUserWorkspaceRoleOwner(userId, workspaceId);

/**
 * @property {BigInt} userId
 * @property {BigInt} workspaceId
 */
const isUserCanDeleteWorkspaceAsync = async (userId, workspaceId) => isUserWorkspaceRoleOwner(userId, workspaceId);

/**
 * @property {BigInt} userId
 * @property {BigInt} workspaceId
 */
const isUserCanGetBoardsAsync = async (userId, workspaceId) => isUserWorkspaceRoleOwnerOrUser(userId, workspaceId);

/**
 * @property {BigInt} userId
 * @property {BigInt} workspaceId
 */
const isUserCanCreateBoardsAsync = async (userId, workspaceId) => isUserWorkspaceRoleOwner(userId, workspaceId);

/**
 * @property {BigInt} userId
 * @property {BigInt} workspaceId
 */
const isUserCanUpdateBoardsAsync = async (userId, workspaceId) => isUserWorkspaceRoleOwner(userId, workspaceId);

/**
 * @property {BigInt} userId
 * @property {BigInt} workspaceId
 */
const isUserCanDeleteBoardsAsync = async (userId, workspaceId) => isUserWorkspaceRoleOwner(userId, workspaceId);

module.exports.isWorkspaceNameValid = isWorkspaceNameValid;
module.exports.isUserCanGetWorkspaceAsync = isUserCanGetWorkspaceAsync;
module.exports.isUserCanCreateWorkspaceAsync = isUserCanCreateWorkspaceAsync;
module.exports.isUserCanUpdateWorkspaceAsync = isUserCanUpdateWorkspaceAsync;
module.exports.isUserCanDeleteWorkspaceAsync = isUserCanDeleteWorkspaceAsync;
module.exports.isUserCanGetBoardsAsync = isUserCanGetBoardsAsync;
module.exports.isUserCanCreateBoardsAsync = isUserCanCreateBoardsAsync;
module.exports.isUserCanUpdateBoardsAsync = isUserCanUpdateBoardsAsync;
module.exports.isUserCanDeleteBoardsAsync = isUserCanDeleteBoardsAsync;
