import dotenv from 'dotenv';
dotenv.config({ path: '.env' });

import express from 'express';
import { schedule } from 'node-cron';
import { createInstance } from './mongodb-init';
import { sync } from './src/controllers/syncController';
import { USER } from './src/shared/context_information';

const port = process.env.PORT;
const app = express();

app.use(express.json());

const clientStart = createInstance({
  host: process.env.DB_HOST_BEGINER,
  port: process.env.DB_PORT_BEGINER,
  context: process.env.DB_CONTEXT_BEGINER,
});

const clientDestination = createInstance({
  host: process.env.DB_HOST_DESTINATION,
  port: process.env.DB_PORT_DESTINATION,
  context: process.env.DB_CONTEXT_DESTINATION,
});

app.post('/sync-manual', async (req, res) => {
  try {
    await sync(
      {
        nameCollection: USER.WRITE_COLLECTION,
        context: process.env.DB_CONTEXT_BEGINER,
        client: clientStart,
      },
      {
        nameCollection: USER.READ_COLLECTION,
        context: process.env.DB_CONTEXT_DESTINATION,
        client: clientDestination,
      },
    );

    res.send({ response: true });
  } catch (ex) {
    console.error(ex);
    res.status(500).send({ response: false });
  }
});

app.listen(port, async () => {
  try {
    await clientStart.connect();
    await clientStart.db(process.env.DB_CONTEXT_BEGINER).command({ ping: 1 });
    console.info('connected database 1');

    await clientDestination.connect();
    await clientDestination.db(process.env.DB_CONTEXT_DESTINATION).command({ ping: 1 });
    console.info('connected database 2');
  } catch (ex) {
    console.error('Database cannot connect', (ex as Error).message);
  }

  console.log(`🚀 Express is listening at http://127.0.0.1:${port}`);
});

schedule('*/15 * * * *', async () => {
  try {
    await sync(
      {
        nameCollection: USER.WRITE_COLLECTION,
        context: process.env.DB_CONTEXT_BEGINER,
        client: clientStart,
      },
      {
        nameCollection: USER.READ_COLLECTION,
        context: process.env.DB_CONTEXT_DESTINATION,
        client: clientDestination,
      },
    );
    console.log('Sync success');
  } catch (ex) {
    console.error('Sync failed', (ex as Error).message);
  }
});
