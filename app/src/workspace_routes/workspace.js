const { Router } = require('express');

const workspaceGet = require('./workspaceGet');
const workspaceGetById = require('./workspaceGetById');
const workspaceCreate = require('./workspaceCreate');
const workspaceUpdate = require('./workspaceUpdate');
const workspaceDelete = require('./workspaceDelete');

const { authHelper } = require('../common/helpers');

const router = Router();

router.use(authHelper.authorizationRequire);
router.post('/workspace.get', workspaceGet);
router.post('/workspace.getById', workspaceGetById);
router.post('/workspace.create', workspaceCreate);
router.post('/workspace.update', workspaceUpdate);
router.post('/workspace.delete', workspaceDelete);

module.exports = router;
