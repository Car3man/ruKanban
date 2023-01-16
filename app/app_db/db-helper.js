const dbPool = require("./db-pool")

const helloWorldQuery = new Promise((res, rej) => {
    dbPool.query("SELECT $1::text as name", ["Hello world"], (err, result) => {
        if (err) {
            return rej(err)
        }
        return res(result)
    })
})

module.exports = { helloWorldQuery }