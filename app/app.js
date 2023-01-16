const express = require("express")
const auth = require("./app_routes/auth")
const throw404 = require("./throw404")

const app = express()
const port = 80

app.use(express.urlencoded({ extended: false }))
app.use(express.json())

app.use("/auth", auth)
app.all("*", throw404)

const server = app.listen(port, (error) => {
    if (error) {
        return console.log(`Error: ${error}`)
    }
    console.log(`Server listening on port ${server.address().port}`)
});