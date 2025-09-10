using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ApiWriteUser.Business;
using ApiWriteUser.Repositories;
using ApiWriteUser.Services;

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

app.MapPost("/user", async (UserCreate request, IUserService service) =>
{
    var user = await service.CreateAsync(request);
    return Results.Ok(new { response = user });
});

app.MapPut("/user/{id}", async (string id, UserCreate request, IUserService service) =>
{
    var result = await service.UpdateAsync(id, request);
    return result is null ? Results.NotFound() : Results.Ok(new { response = result });
});

app.MapDelete("/user/{id}", async (string id, IUserService service) =>
{
    var deleted = await service.DeleteAsync(id);
    return Results.Ok(new { response = deleted });
});

app.Run();

