const { Router } = require('express');

const authSignUp = require('./authSignUp');
const authSignIn = require('./authSignIn');
const authSignOut = require('./authSignOut');
const authChangePassword = require('./authChangePassword');
const authRefreshTokens = require('./authRefreshTokens');

const { authHelper } = require('../common/helpers');

const router = Router();

router.post('/auth.signUp', authHelper.unauthorizationRequire, authSignUp);
router.post('/auth.signIn', authHelper.unauthorizationRequire, authSignIn);
router.post('/auth.signOut', authHelper.authorizationRequire, authSignOut);
router.post('/auth.changePassword', authHelper.authorizationRequire, authChangePassword);
router.post('/auth.refreshTokens', authRefreshTokens);

module.exports = router;
