using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using ApiSyncUser.Models;

var builder = WebApplication.CreateBuilder(args);

var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "27017";
var context = Environment.GetEnvironmentVariable("DB_CONTEXT") ?? "user";

var connectionString = $"mongodb://{host}:{port}/?authSource={context}";
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(context));

var app = builder.Build();

var db = app.Services.GetRequiredService<IMongoDatabase>();
var readCollection = db.GetCollection<User>("user_read");
var writeCollection = db.GetCollection<User>("user_write");

app.MapPost("/sync-manual", async () =>
{
    var unsynced = await writeCollection.Find(u => !u.IsSync).ToListAsync();
    if (unsynced.Count > 0)
    {
        unsynced.ForEach(u =>
        {
            u.IsSync = true;
            u.DateSynced = DateTime.UtcNow;
        });

        await readCollection.InsertManyAsync(unsynced);

        var ids = unsynced.Select(u => u.Id).ToList();
        var update = Builders<User>.Update.Set(u => u.IsSync, true);
        await writeCollection.UpdateManyAsync(u => ids.Contains(u.Id), update);
    }

    return Results.Ok(new { response = true });
});

app.Run();

