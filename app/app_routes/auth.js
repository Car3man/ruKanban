const { Router } = require('express');
const { sendNotFound } = require('../common/response-helper');
const {
  authorizationRequireMiddleware,
  unauthorizationRequireMiddleware,
} = require('../common/auth-helper');
const {
  signUp, signIn, signOut, changePassword, refreshToken,
} = require('./auth-controller');

const router = Router();

router.post('/signUp', unauthorizationRequireMiddleware, signUp);
router.post('/signIn', unauthorizationRequireMiddleware, signIn);
router.post('/signOut', authorizationRequireMiddleware, signOut);
router.post('/changePassword', authorizationRequireMiddleware, changePassword);
router.post('/refreshToken', authorizationRequireMiddleware, refreshToken);
router.all('*', sendNotFound);

module.exports = router;
