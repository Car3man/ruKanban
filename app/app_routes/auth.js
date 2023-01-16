const { Router } = require("express")
const throw404 = require("../throw404")
const { signUp, signIn, signOut, changePassword } = require("./auth-controller")

const router = Router()

router.post("/signUp", signUp)
router.post("/signIn", signIn)
router.post("/signOut", signOut)
router.post("/changePassword", changePassword)
router.all("*", throw404)

module.exports = router