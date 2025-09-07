using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ApiWriteUser.Models;

var builder = WebApplication.CreateBuilder(args);

var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "27017";
var context = Environment.GetEnvironmentVariable("DB_CONTEXT") ?? "user";

var connectionString = $"mongodb://{host}:{port}/?authSource={context}";
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(context));

var app = builder.Build();

var db = app.Services.GetRequiredService<IMongoDatabase>();
var writeCollection = db.GetCollection<User>("user_write");

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

app.Run();

