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

router.use('/', authorizationRequireAsync);

router.get('/', listWorkspaces);
router.get('/:id', getWorkspace);
router.post('/', createWorkspace);
router.put('/:id', updateWorkspace);
router.delete('/:id', deleteWorkspace);
router.all('*', sendNotFound);

module.exports = router;
