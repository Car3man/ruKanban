const { Router } = require('express');

const columnGet = require('./columnGet');
const columnGetById = require('./columnGetById');
const columnCreate = require('./columnCreate');
const columnUpdate = require('./columnUpdate');
const columnDelete = require('./columnDelete');

const { authHelper } = require('../common/helpers');

const router = Router();

router.use(authHelper.authorizationRequire);
router.post('/column.get', columnGet);
router.post('/column.getById', columnGetById);
router.post('/column.create', columnCreate);
router.post('/column.update', columnUpdate);
router.post('/column.delete', columnDelete);

module.exports = router;
