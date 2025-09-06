import dotenv from 'dotenv'
dotenv.config({ path: '.env'});

import express from 'express';
import { client } from './mongodb-init';
import { createUser, updateUser, deleteUser } from './src/controllers/userController';
import { UserCreate, UserFilter } from './src/interfaces/user.interface';

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

app.put('/users', async (req,res) => {
  // const { username, email } = req.query;
  // const pageSize = req.query.pageSize ? parseInt(req.query.pageSize as string) : 0;
  // const pageNumb =  req.query.pageNumber ? parseInt(req.query.pageNumber as string) : 0;

  // const userQuery: UserFilter = {
  //   username: username as string,
  //   email: email as string,
  // };

  // const result = await getUsers({ pageNumber: pageNumb, pageSize: pageSize, }, { ...userQuery });
  // res.send({
  //   response: result,
  //   pageSize: pageSize,
  //   pageNumber: pageNumb
  // });
});

app.delete('/users/:id', async (req,res) => {

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
