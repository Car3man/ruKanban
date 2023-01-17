const { Router } = require("express")
const { sendNotFound } = require("../common/response-helper")
const { signUp, signIn, signOut, changePassword } = require("./auth-controller")

const router = Router()

router.post("/signUp", signUp)
router.post("/signIn", signIn)
router.post("/signOut", signOut)
router.post("/changePassword", changePassword)
router.all("*", sendNotFound)

module.exports = router