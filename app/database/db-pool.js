const { host, user, password, database, port } = require("./db-config")
const { Pool } = require("pg")

const pool = new Pool({
    host: host,
    user: user,
    password: password,
    database: database,
    max: 20,
    idleTimeoutMillis: 30000,
    connectionTimeoutMillis: 2000,
})

module.exports = pool