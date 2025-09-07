using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ApiReadUsers.Business;
using ApiReadUsers.Repositories;
using ApiReadUsers.Services;

var builder = WebApplication.CreateBuilder(args);

var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "27017";
var context = Environment.GetEnvironmentVariable("DB_CONTEXT") ?? "user";

var connectionString = $"mongodb://{host}:{port}/?authSource={context}";
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(context));
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IUserService, UserService>();

var app = builder.Build();

app.MapGet("/user/{id}", async (string id, IUserService service) =>
{
    var user = await service.GetUserAsync(id);
    return user is null ? Results.NotFound() : Results.Ok(new { response = user });
});

app.MapGet("/users", async (
    [FromQuery] string? username,
    [FromQuery] string? email,
    [FromQuery] int pageSize,
    [FromQuery] int pageNumber,
    IUserService service) =>
{
    var result = await service.GetUsersAsync(username, email, pageSize, pageNumber);
    return Results.Ok(new { response = result.Users, pageSize = result.PageSize, pageNumber = result.PageNumber });
});

app.Run();

