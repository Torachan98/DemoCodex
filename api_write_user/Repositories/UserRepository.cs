using ApiWriteUser.Business;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ApiWriteUser.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _writeCollection;

    public UserRepository(IMongoDatabase db)
    {
        _writeCollection = db.GetCollection<User>("user_write");
    }

    public Task InsertAsync(User user)
    {
        return _writeCollection.InsertOneAsync(user);
    }

    public async Task<User?> UpdateAsync(ObjectId id, UserCreate request)
    {
        var update = Builders<User>.Update
            .Set(u => u.Username, request.Username)
            .Set(u => u.Email, request.Email)
            .Set(u => u.IsSync, false)
            .Set(u => u.DateUpdated, DateTime.UtcNow);

        return await _writeCollection.FindOneAndUpdateAsync(
            u => u.Id == id,
            update,
            new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After });
    }

    public async Task<bool> DeleteAsync(ObjectId id)
    {
        var result = await _writeCollection.DeleteOneAsync(u => u.Id == id);
        return result.DeletedCount > 0;
    }
}
