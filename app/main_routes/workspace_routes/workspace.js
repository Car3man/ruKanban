const { Router } = require('express');
const { sendNotFound } = require('../../common/response-helper');
const { authorizationRequireAsync } = require('../../common/auth-helper');
const {
  listWorkspaces,
  getWorkspace,
  createWorkspace,
  updateWorkspace,
  deleteWorkspace,
} = require('./workspace-controller');
const board = require('./board_routes/board');

const router = Router();

router.use('/', authorizationRequireAsync);

router.get('/', listWorkspaces);
router.get('/:id', getWorkspace);
router.post('/', createWorkspace);
router.put('/:id', updateWorkspace);
router.delete('/:id', deleteWorkspace);

router.use('/:id/board', board);
router.all('*', sendNotFound);

module.exports = router;
