const { PrismaClient } = require('@prisma/client');
const crypto = require('crypto');
const app = require('./src/app');

const prisma = new PrismaClient();
const port = 80;

/**
 * Initialize the server
 * @async
 */
async function initServer() {
  const jwtSecret = await prisma.jwt_secrets.findFirst();
  if (jwtSecret) {
    global.jwtSecret = jwtSecret.secret;
  } else {
    global.jwtSecret = crypto.randomBytes(64).toString('hex');
    await prisma.jwt_secrets.create({
      data: {
        secret: global.jwtSecret,
        created_at: new Date(),
      },
    });
  }

  console.log(`Server will use next jwt secret: ${global.jwtSecret}`);

  const server = app.listen(port, (error) => {
    if (error) {
      return console.log(`Server initialization error: ${error}`);
    }
    return console.log(`Server initialized and started on port ${server.address().port}`);
  });
}

initServer();
