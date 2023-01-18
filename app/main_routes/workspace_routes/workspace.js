const { Router } = require('express');
const { sendNotFound } = require('../../common/response-helper');
const { authorizationRequireAsync } = require('../../common/auth-helper');
const {
  getWorkspace,
  createWorkspace,
  updateWorkspace,
  deleteWorkspace,
  listWorkspaces,
} = require('./workspace-controller');

const router = Router();

router.get('/:id', authorizationRequireAsync, getWorkspace);
router.post('/', authorizationRequireAsync, createWorkspace);
router.put('/:id', authorizationRequireAsync, updateWorkspace);
router.delete('/:id', authorizationRequireAsync, deleteWorkspace);
router.get('/', authorizationRequireAsync, listWorkspaces);
router.all('*', sendNotFound);

module.exports = router;
