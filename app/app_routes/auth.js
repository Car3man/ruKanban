const { Router } = require("express")
const throw404 = require("../throw404")
const { signUp, signIn, signOut, changePassword } = require("./auth-controller")

const router = Router()

router.get("/signUp", signUp)
router.get("/signIn", signIn)
router.get("/signOut", signOut)
router.get("/changePassword", changePassword)
router.all("*", throw404)

module.exports = router