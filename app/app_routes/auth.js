const { Router } = require('express');
const { sendNotFound } = require('../common/response-helper');
const {
  authorizationRequireAsync,
  unauthorizationRequireAsync,
} = require('../common/auth-helper');
const {
  signUp, signIn, signOut, changePassword, refreshTokens,
} = require('./auth-controller');

const router = Router();

router.post('/signUp', unauthorizationRequireAsync, signUp);
router.post('/signIn', unauthorizationRequireAsync, signIn);
router.post('/signOut', authorizationRequireAsync, signOut);
router.post('/changePassword', authorizationRequireAsync, changePassword);
router.post('/refreshTokens', refreshTokens);
router.all('*', sendNotFound);

module.exports = router;
