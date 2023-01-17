const { PrismaClient } = require("@prisma/client")
const md5 = require("md5")
const authHelper = require("../common/auth-helper")
const responseHelper = require("../common/response-helper")

const prisma = new PrismaClient()
const getRefreshTokenExpiresAt = () => Date.now() + (1000 * 60 * 60 * 24 * 30 * 3)

const signUp = async (req, res) => {
    const login = req.body.login
    const password = req.body.password
    const firstName = req.body.first_name
    const surName = req.body.sur_name
    const patronymic = req.body.patronymic

    try {
        const isUserExist = await prisma.users.count({
            where: { login: login }
        }) > 0

        if (isUserExist) {
            return responseHelper.sendBadRequest(req, res, {
                extended_msg: "The user with same login exist."
            })
        }
    } catch (err) {
        console.log(err)
        return responseHelper.sendInternalServerError(req, res)
    }

    const userData = {
        login: login,
        password_hash: md5(password),
        first_name: firstName,
        sur_name: surName,
        patronymic: patronymic,
        roles: {
            connect: { name: "user" }
        },
        created_at: new Date()
    }

    try {
        await prisma.users.create({ data: userData })
    } catch (err) {
        console.log(err)
        return responseHelper.sendInternalServerError(req, res)
    }

    const accessToken = authHelper.createAccessToken(login)
    const refreshTokenExpiresAt = getRefreshTokenExpiresAt()
    const refreshToken = authHelper.createRefreshToken(accessToken, refreshTokenExpiresAt)

    try {
        await prisma.refresh_tokens.create({
            data: {
                token: refreshToken,
                users: {
                    connect: { login: login }
                },
                created_at: new Date(),
                expires_at: new Date(refreshTokenExpiresAt)
            }
        })
    } catch (err) {
        console.log(err)
        return responseHelper.sendInternalServerError(req, res)
    }

    res.status(200).send({
        accessToken: accessToken,
        refreshToken: refreshToken
    })
};

const signIn = async (req, res) => {
    const login = req.body.login
    const password = req.body.password

    let userData;

    try {
        userData = await prisma.users.findFirst({
            where: { login: login }
        })

        if (!userData) {
            return responseHelper.sendBadRequest(req, res, {
                extended_msg: "The login or password is wrong."
            })
        }
    } catch (err) {
        console.log(err)
        return responseHelper.sendInternalServerError(req, res)
    }

    if (userData.password_hash !== md5(password)) {
        return responseHelper.sendBadRequest(req, res, {
            extended_msg: "The login or password is wrong."
        })
    }

    const accessToken = authHelper.createAccessToken(login)
    const refreshTokenExpiresAt = getRefreshTokenExpiresAt()
    const refreshToken = authHelper.createRefreshToken(accessToken, refreshTokenExpiresAt)

    try {
        await prisma.refresh_tokens.create({
            data: {
                token: refreshToken,
                users: {
                    connect: { login: login }
                },
                created_at: new Date(),
                expires_at: new Date(refreshTokenExpiresAt)
            }
        })
    } catch (err) {
        console.log(err)
        return responseHelper.sendInternalServerError(req, res)
    }

    res.status(200).send({
        accessToken: accessToken,
        refreshToken: refreshToken
    })
};

const signOut = async (req, res) => {
    const accessToken = req.accessToken

    try {
        await prisma.revoked_tokens.create({
            data: {
                token: accessToken,
                revoked_at: new Date()
            }
        })
    } catch (err) {
        console.log(err)
        return responseHelper.sendInternalServerError(req, res)
    }

    return responseHelper.sendOk(req, res)
};

const changePassword = async (req, res) => {
    const login = req.login
    const accessToken = req.accessToken
    const currentPassword = req.body.currentPassword
    const newPassword = req.body.newPassword

    let userData;

    try {
        userData = await prisma.users.findFirst({
            where: { login: login }
        })

        if (!userData) {
            return responseHelper.sendInternalServerError(req, res)
        }
    } catch (err) {
        console.log(err)
        return responseHelper.sendInternalServerError(req, res)
    }

    if (userData.password_hash !== md5(currentPassword)) {
        return responseHelper.sendBadRequest(req, res, {
            extended_msg: "Current password doesn't match actual."
        })
    }

    try {
        await prisma.users.update({
            where: {
                login: login
            },
            data: {
                password_hash: md5(newPassword)
            }
        })
    } catch (err) {
        console.log(err)
        return responseHelper.sendInternalServerError(req, res)
    }

    try {
        await prisma.revoked_tokens.create({
            data: {
                token: accessToken,
                revoked_at: new Date()
            }
        })
    } catch (err) {
        console.log(err)
        return responseHelper.sendInternalServerError(req, res)
    }

    return responseHelper.sendOk(req, res)
};

module.exports = { signUp, signIn, signOut, changePassword }