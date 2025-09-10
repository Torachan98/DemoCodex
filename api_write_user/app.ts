import dotenv from 'dotenv'
dotenv.config({ path: '.env'});

import express from 'express';
import { client } from './mongodb-init';
import { createUser, updateUser, deleteUser } from './src/controllers/userController';
import { User, UserCreate, UserFilter } from './src/interfaces/user.interface';

const port = 3001;
const app = express();
app.use(express.json());

app.post('/user', async (req,res) => {
  const request = req.body as UserCreate;
  const result = await createUser(request);
  res.send({
    response: result
  });
});

// Update user by id
app.put('/users/:id', async (req,res) => {
  try {
    const { id } = req.params;
    const payload = req.body as Partial<User>;

    // The controller expects a full User shape; cast for now
    const result = await updateUser(id, payload as User);
    res.send({ response: result });
  } catch (err: any) {
    res.status(500).send({ error: err?.message ?? 'Unexpected error' });
  }
});

// Delete user by id
app.delete('/users/:id', async (req,res) => {
  try {
    const { id } = req.params;
    const result = await deleteUser(id);
    res.send({ response: result });
  } catch (err: any) {
    res.status(500).send({ error: err?.message ?? 'Unexpected error' });
  }
});

app.listen(port, async () => {
  try {    
    await client.connect();
    await client.db(process.env.DB_CONTEXT).command({ ping: 1 });
    console.log(`Database connected`);
  } catch (ex) {
    console.error(`Database cannot connect`, ex.message);
  } finally {
    await client.close();
  }

  console.log(`🚀 Express is listening at http://127.0.0.1:${port}`);
});
