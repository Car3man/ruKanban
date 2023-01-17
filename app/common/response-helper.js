const httpCodes = require('./http-codes');

module.exports.sendOk = (req, res, data) => {
  res.status(httpCodes.ok).send(data || 'OK');
};

module.exports.sendBadRequest = (req, res, data) => {
  res.status(httpCodes.badRequest).send(data || 'The request was unacceptable, often due to missing a required parameter.');
};

module.exports.sendUnauthorized = (req, res, data) => {
  res.status(httpCodes.unauthorized).send(data || 'The authorization required.');
};

module.exports.sendForbidden = (req, res, data) => {
  res.status(httpCodes.unauthorized).send(data || "The user doesn't have permissions to perform the request.");
};

module.exports.sendNotFound = (req, res, data) => {
  res.status(httpCodes.notFound).send(data || "The requested resource doesn't exist.");
};

module.exports.sendInternalServerError = (req, res, data) => {
  res.status(httpCodes.internalServerError).send(data || 'Internal server error, we are working on the problem.');
};
