module.exports.escapeBigInt = (object) => {
  const escapedObject = object;
  Object.keys(object).forEach((key) => {
    escapedObject[key] = typeof object[key] === 'bigint' ? object[key].toString() : object[key];
  });
  return escapedObject;
};
