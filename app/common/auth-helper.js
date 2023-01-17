const jwt = require("jsonwebtoken")
const responseHelper = require("../common/response-helper")

module.exports.unauthorizedRequiredMiddleware = (req, res, next) => {
    const authorizationHeader = req.headers.authorization

    if (authorizationHeader) {
        return responseHelper.sendBadRequest(req, res, {
            extended_msg: `Sign out before use ${req.path} method`
        })
    }
    next()
}

module.exports.authenticateMiddleware = (req, res, next) => {
    const authorizationHeader = req.headers.authorization

    if (authorizationHeader) {
        const token = authorizationHeader.split(' ')[1]

        jwt.verify(token, process.env.JWT_SECRET, (err, { login }) => {
            if (err) {
                return responseHelper.sendForbidden(req, res)
            }

            req.login = login
            next()
        });
    }
    return responseHelper.sendUnauthorized(req, res)
}

module.exports.createAccessToken = (login) => {
    return jwt.sign({
        iss: process.env.JWT_ISSUER,
        aud: process.env.JWT_AUDIENCE,
        exp: 1000 + (1000 * 60 * 60 * 5),
        alg: "HS256",
        login: login
    }, process.env.JWT_SECRET);
}

module.exports.createRefreshToken = (accessToken, expiresAt) => {
    return jwt.sign({
        iss: process.env.JWT_ISSUER,
        aud: process.env.JWT_AUDIENCE,
        exp: expiresAt,
        alg: "HS256",
        accessToken: accessToken
    }, process.env.JWT_SECRET);
}