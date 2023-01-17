const express = require("express")
const auth = require("./routes/auth")
const { sendNotFound } = require("./common/response-helper")
require('dotenv').config()

const app = express()
const port = 80

app.use(express.urlencoded({ extended: false }))
app.use(express.json())

app.use("/auth", auth)
app.all("*", sendNotFound)

const server = app.listen(port, (error) => {
    if (error) {
        return console.log(`Error: ${error}`)
    }
    console.log(`Server listening on port ${server.address().port}`)
});