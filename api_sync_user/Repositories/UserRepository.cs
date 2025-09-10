using ApiSyncUser.Business;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ApiSyncUser.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _readCollection;
    private readonly IMongoCollection<User> _writeCollection;

    public UserRepository(IMongoDatabase db)
    {
        _readCollection = db.GetCollection<User>("user_read");
        _writeCollection = db.GetCollection<User>("user_write");
    }

    public Task<List<User>> GetUnsyncedAsync()
    {
        return _writeCollection.Find(u => !u.IsSync).ToListAsync();
    }

    public Task InsertReadAsync(IEnumerable<User> users)
    {
        return _readCollection.InsertManyAsync(users);
    }

    public Task MarkSyncedAsync(IEnumerable<ObjectId> ids)
    {
        var update = Builders<User>.Update
            .Set(u => u.IsSync, true)
            .Set(u => u.DateSynced, DateTime.UtcNow);
        return _writeCollection.UpdateManyAsync(u => ids.Contains(u.Id), update);
    }
}
