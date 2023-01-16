const httpCodes = require("./http-codes")

module.exports.sendOk = (req, res, data) => {
    res.status(httpCodes.ok).send(data)
}

module.exports.sendBadRequest = (req, res) => {
    res.status(httpCodes.badRequest).send("The request was unacceptable, often due to missing a required parameter.")
}

module.exports.sendUnauthorized = (req, res) => {
    res.status(httpCodes.unauthorized).send("The authorization required.")
}

module.exports.sendForbidden = (req, res) => {
    res.status(httpCodes.unauthorized).send("The user doesn't have permissions to perform the request.")
}

module.exports.sendNotFound = (req, res) => {
    res.status(httpCodes.notFound).send("The requested resource doesn't exist.")
}