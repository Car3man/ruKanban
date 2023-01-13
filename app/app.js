import express from "express"

const port = 80
const app = express()

app.get('/', (request, response) => {
    console.log(`URL: ${request.url}`)
    response.send('Hello, Server!')
});

const server = app.listen(port, (error) => {
    if (error) return console.log(`Error: ${error}`)
    console.log(`Server listening on port ${server.address().port}`)
});