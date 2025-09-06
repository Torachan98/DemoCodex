    const db = client.db('management_context');         // Your DB name
    const collection = db.collection('doiknowyou');

    const newUser = {
      username: 'baoz.vo',
      email: 'bao.vo@orientsoftware.com',
      age: 30,
      is_active: true
    };

    const resultInsert = await collection.insertOne(newUser);
    console.log(JSON.stringify(resultInsert));
    const result = await collection.find({}).toArray();
    console.log(JSON.stringify(result));