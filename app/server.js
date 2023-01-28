const app = require('./src/app');

const port = 80;

const server = app.listen(port, (error) => {
  if (error) {
    return console.log(`Error: ${error}`);
  }
  return console.log(`Server listening on port ${server.address().port}`);
});
