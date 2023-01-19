const { Router } = require('express');
const { sendNotFound } = require('../../../common/response-helper');
const { authorizationRequireAsync } = require('../../../common/auth-helper');
const {
  listBoards,
  getBoard,
  createBoard,
  updateBoard,
  deleteBoard,
} = require('./board-controller');

const router = Router();

router.use('/', authorizationRequireAsync);

router.get('/', listBoards);
router.get('/:id', getBoard);
router.post('/', createBoard);
router.put('/:id', updateBoard);
router.delete('/:id', deleteBoard);
router.all('*', sendNotFound);

module.exports = router;
