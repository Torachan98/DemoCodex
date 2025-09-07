using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ApiReadUsers.Models;

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

app.Run();

