using Microsoft.AspNetCore.Mvc;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using UserApi.Models;

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

app.MapGet("/user/{id}", async (string id) =>
{
    if (!ObjectId.TryParse(id, out var objectId))
        return Results.NotFound();

    var user = await readCollection.Find(u => u.Id == objectId).FirstOrDefaultAsync();
    return Results.Ok(new { response = user });
});

app.MapGet("/users", async (
    [FromQuery] string? username,
    [FromQuery] string? email,
    [FromQuery] int pageSize = 10,
    [FromQuery] int pageNumber = 1) =>
{
    var filter = Builders<User>.Filter.Empty;
    if (!string.IsNullOrEmpty(username))
        filter &= Builders<User>.Filter.Eq(u => u.Username, username);
    if (!string.IsNullOrEmpty(email))
        filter &= Builders<User>.Filter.Eq(u => u.Email, email);

    if (pageSize <= 0) pageSize = 10;
    if (pageNumber <= 0) pageNumber = 1;

    var users = await readCollection.Find(filter)
        .Skip((pageNumber - 1) * pageSize)
        .Limit(pageSize)
        .ToListAsync();

    return Results.Ok(new { response = users, pageSize, pageNumber });
});

app.MapPost("/user", async (UserCreate request) =>
{
    var user = new User
    {
        Id = ObjectId.GenerateNewId(),
        Username = request.Username,
        Email = request.Email,
        IsActive = true,
        IsSync = false,
        DateCreated = DateTime.UtcNow,
        DateUpdated = DateTime.UtcNow
    };

    await writeCollection.InsertOneAsync(user);
    return Results.Ok(new { response = user });
});

app.MapPut("/user/{id}", async (string id, UserCreate request) =>
{
    if (!ObjectId.TryParse(id, out var objectId))
        return Results.NotFound();

    var update = Builders<User>.Update
        .Set(u => u.Username, request.Username)
        .Set(u => u.Email, request.Email)
        .Set(u => u.IsSync, false)
        .Set(u => u.DateUpdated, DateTime.UtcNow);

    var result = await writeCollection.FindOneAndUpdateAsync<User>(
        u => u.Id == objectId,
        update,
        new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After });

    if (result is null)
        return Results.NotFound();

    return Results.Ok(new { response = result });
});

app.MapDelete("/user/{id}", async (string id) =>
{
    if (!ObjectId.TryParse(id, out var objectId))
        return Results.NotFound();

    var result = await writeCollection.DeleteOneAsync(u => u.Id == objectId);
    return Results.Ok(new { response = result.DeletedCount > 0 });
});

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

