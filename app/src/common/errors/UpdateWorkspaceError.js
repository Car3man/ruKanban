/* eslint-disable no-unused-vars */

class UpdateWorkspaceError extends Error {
  constructor(message) {
    super(message);
    this.name = 'UpdateWorkspaceError';
  }
}
