import dotenv from 'dotenv'
dotenv.config({ path: '.env'});

import express from 'express';
import { client } from './mongodb-init';
import { getOneUser, getUsers } from './src/controllers/userController';
import { UserFilter } from './src/interfaces/user.interface';

const port = 3000;
const app = express();
app.use(express.json());

app.get('/user/:id', async (req,res) => {
  const result = await getOneUser(req.params.id);
  res.send({
    response: result
  });
});

app.get('/users', async (req, res) => {
  const { username, email } = req.query;
  const pageSize = req.query.pageSize
    ? parseInt(req.query.pageSize as string, 10)
    : 10;
  const pageNumber = req.query.pageNumber
    ? parseInt(req.query.pageNumber as string, 10)
    : 1;

  const userQuery: UserFilter = {
    username: username as string,
    email: email as string,
  };

  const result = await getUsers(
    { pageNumber, pageSize },
    { ...userQuery },
  );
  res.send({
    response: result,
    pageSize,
    pageNumber,
  });
});

app.listen(port, async () => {
  try {
    await client.connect();
    await client.db(process.env.DB_CONTEXT).command({ ping: 1 });
    console.log('Database connected');
  } catch (ex) {
    console.error('Database cannot connect', (ex as Error).message);
  }

  console.log(`🚀 Express is listening at http://127.0.0.1:${port}`);
});
