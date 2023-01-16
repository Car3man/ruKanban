const { Router } = require("express")
const { sendNotFound } = require("../common/response-helper")
const { signUp, signIn, signOut, changePassword } = require("./auth-controller")

const router = Router()

router.get("/signUp", signUp)
router.get("/signIn", signIn)
router.get("/signOut", signOut)
router.get("/changePassword", changePassword)
router.all("*", sendNotFound)

module.exports = router