/* eslint-disable no-unused-vars */

class UpdateBoardError extends Error {
  constructor(message) {
    super(message);
    this.name = 'UpdateBoardError';
  }
}
