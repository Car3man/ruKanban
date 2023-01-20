const { Router } = require('express');

const boardGet = require('./boardGet');
const boardGetById = require('./boardGetById');
const boardCreate = require('./boardCreate');
const boardUpdate = require('./boardUpdate');
const boardDelete = require('./boardDelete');

const { authHelper } = require('../common/helpers');

const router = Router();

router.use(authHelper.authorizationRequire);
router.post('/board.get', boardGet);
router.post('/board.getById', boardGetById);
router.post('/board.create', boardCreate);
router.post('/board.update', boardUpdate);
router.post('/board.delete', boardDelete);

module.exports = router;
