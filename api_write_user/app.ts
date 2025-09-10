import dotenv from 'dotenv'
dotenv.config({ path: '.env'});

import express from 'express';
import { client } from './mongodb-init';
import { createUser, updateUser, deleteUser } from './src/controllers/userController';
import { User, UserCreate, UserFilter } from './src/interfaces/user.interface';
import handle from './src/shared/response-handler';

const port = 3001;
const app = express();
app.use(express.json());

app.post(
  '/user',
  handle(async (req) => {
    const request = req.body as UserCreate;
    return await createUser(request);
  })
);

// Update user by id
app.put(
  '/users/:id',
  handle(async (req) => {
    const { id } = req.params;
    const payload = req.body as Partial<User>;
    return await updateUser(id, payload as User);
  })
);

// Delete user by id
app.delete(
  '/users/:id',
  handle(async (req) => {
    const { id } = req.params;
    return await deleteUser(id);
  })
);

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
