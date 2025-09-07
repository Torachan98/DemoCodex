using MongoDB.Driver;
using ApiSyncUser.Business;
using ApiSyncUser.Repositories;
using ApiSyncUser.Services;

var builder = WebApplication.CreateBuilder(args);

var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "27017";
var context = Environment.GetEnvironmentVariable("DB_CONTEXT") ?? "user";

var connectionString = $"mongodb://{host}:{port}/?authSource={context}";
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(context));
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<ISyncService, SyncService>();

var app = builder.Build();

app.MapPost("/sync-manual", async (ISyncService service) =>
{
    await service.SyncAsync();
    return Results.Ok(new { response = true });
});

app.Run();

