const listBoards = require('./board_controller/listBoards');
const getBoard = require('./board_controller/getBoard');
const createBoard = require('./board_controller/createBoard');
const updateBoard = require('./board_controller/updateBoard');
const deleteBoard = require('./board_controller/deleteBoard');

module.exports.listBoards = listBoards;
module.exports.getBoard = getBoard;
module.exports.createBoard = createBoard;
module.exports.updateBoard = updateBoard;
module.exports.deleteBoard = deleteBoard;
