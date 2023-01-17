const { Router } = require("express")
const { sendNotFound } = require("../common/response-helper")
const { unauthorizedRequiredMiddleware, authenticateMiddleware } = require("../common/auth-helper")
const { signUp, signIn, signOut, changePassword } = require("./auth-controller")

const router = Router()

router.post("/signUp", unauthorizedRequiredMiddleware, signUp)
router.post("/signIn", unauthorizedRequiredMiddleware, signIn)
router.post("/signOut", authenticateMiddleware, signOut)
router.post("/changePassword", authenticateMiddleware, changePassword)
router.all("*", sendNotFound)

module.exports = router