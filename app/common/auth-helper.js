const jwt = require("jsonwebtoken")

const jwtSecret = process.env.JWT_SECRET
const jwtAudience = process.env.JWT_AUDIENCE
const jwtIssuer = process.env.JWT_ISSUER

module.exports.createAccessToken = (login) => {
    return jwt.sign({
        iss: jwtIssuer,
        aud: jwtAudience,
        exp: 1000 + (1000 * 60 * 60 * 5),
        alg: "HS256",
        login: login
    }, jwtSecret);
}

module.exports.createRefreshToken = (accessToken, expiresAt) => {
    return jwt.sign({
        iss: jwtIssuer,
        aud: jwtAudience,
        exp: expiresAt,
        alg: "HS256",
        accessToken: accessToken
    }, jwtSecret);
}