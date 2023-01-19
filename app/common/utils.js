/**
 * Transforms BigInt object properties into String
 * @param {Object} object
 * @returns {Object}
 */
module.exports.escapeObjectBigInt = (object) => {
  const escapedObject = object;
  Object.keys(object).forEach((key) => {
    escapedObject[key] = typeof object[key] === 'bigint' ? object[key].toString() : object[key];
  });
  return escapedObject;
};

/**
 * Transforms BigInt array into String array
 * @param {Array<BigInt>} array
 * @returns {Array<string>}
 */
module.exports.escapeArrayBigInt = (array) => array.map((x) => x.toString());
