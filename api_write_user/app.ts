import dotenv from 'dotenv'
dotenv.config({ path: '.env'});

import express from 'express';
import { client } from './mongodb-init';
import { createUser, updateUser, deleteUser } from './src/controllers/userController';
import { User, UserCreate } from './src/interfaces/user.interface';

const port = 3001;
const app = express();
app.use(express.json());

app.post('/user', async (req, res) => {
  const request = req.body as UserCreate;
  const result = await createUser(request);
  res.send({
    response: result,
  });
});

app.put('/user/:id', async (req, res) => {
  const result = await updateUser(req.params.id, req.body as User);
  res.send({
    response: result,
  });
});

app.delete('/user/:id', async (req, res) => {
  const result = await deleteUser(req.params.id);
  res.send({
    response: result,
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
