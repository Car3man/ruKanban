const { Router } = require('express');

const ticketGet = require('./ticketGet');
const ticketGetById = require('./ticketGetById');
const ticketCreate = require('./ticketCreate');
const ticketChangeTitle = require('./ticketChangeTitle');
const ticketChangeDescription = require('./ticketChangeDescription');
const ticketMove = require('./ticketMove');
const ticketDelete = require('./ticketDelete');

const { authHelper } = require('../common/helpers');

const router = Router();

router.use(authHelper.authorizationRequire);
router.post('/ticket.get', ticketGet);
router.post('/ticket.getById', ticketGetById);
router.post('/ticket.create', ticketCreate);
router.post('/ticket.changeTitle', ticketChangeTitle);
router.post('/ticket.changeDescription', ticketChangeDescription);
router.post('/ticket.move', ticketMove);
router.post('/ticket.delete', ticketDelete);

module.exports = router;
