const signUp = require('./auth_controller/signUp');
const signIn = require('./auth_controller/signIn');
const signOut = require('./auth_controller/signOut');
const changePassword = require('./auth_controller/changePassword');
const refreshTokens = require('./auth_controller/refreshTokens');

module.exports.signUp = signUp;
module.exports.signIn = signIn;
module.exports.signOut = signOut;
module.exports.changePassword = changePassword;
module.exports.refreshTokens = refreshTokens;
