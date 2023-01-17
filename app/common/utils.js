module.exports.escapeBigInt = (object) => {
    return Object.keys(object).reduce(function (result, key) {
        result[key] = typeof object[key] === 'bigint' ? object[key].toString() : object[key]
        return result
    }, {})
}