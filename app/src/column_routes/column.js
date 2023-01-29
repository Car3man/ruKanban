const { Router } = require('express');

const columnGet = require('./columnGet');
const columnGetById = require('./columnGetById');
const columnCreate = require('./columnCreate');
const columnChangeTitle = require('./columnChangeTitle');
const columnMove = require('./columnMove');
const columnDelete = require('./columnDelete');

const { authHelper } = require('../common/helpers');

const router = Router();

router.use(authHelper.authorizationRequire);
router.post('/column.get', columnGet);
router.post('/column.getById', columnGetById);
router.post('/column.create', columnCreate);
router.post('/column.changeTitle', columnChangeTitle);
router.post('/column.move', columnMove);
router.post('/column.delete', columnDelete);

module.exports = router;
