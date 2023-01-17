const { Router } = require('express');
const { sendNotFound } = require('../common/response-helper');
const {
  authorizationRequireMiddleware,
  unauthorizationRequireMiddleware,
} = require('../common/auth-helper');
const {
  signUp, signIn, signOut, changePassword,
} = require('./auth-controller');

const router = Router();

router.post('/signUp', unauthorizationRequireMiddleware, signUp);
router.post('/signIn', unauthorizationRequireMiddleware, signIn);
router.post('/signOut', authorizationRequireMiddleware, signOut);
router.post('/changePassword', authorizationRequireMiddleware, changePassword);
router.all('*', sendNotFound);

module.exports = router;
