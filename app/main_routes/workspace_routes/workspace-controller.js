const getWorkspace = require('./workspace_controller/getWorkspace');
const createWorkspace = require('./workspace_controller/createWorkspace');
const updateWorkspace = require('./workspace_controller/updateWorkspace');
const deleteWorkspace = require('./workspace_controller/deleteWorkspace');
const listWorkspaces = require('./workspace_controller/listWorkspaces');

module.exports.getWorkspace = getWorkspace;
module.exports.createWorkspace = createWorkspace;
module.exports.updateWorkspace = updateWorkspace;
module.exports.deleteWorkspace = deleteWorkspace;
module.exports.listWorkspaces = listWorkspaces;
