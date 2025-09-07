using ApiReadUsers.Business;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ApiReadUsers.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _readCollection;

    public UserRepository(IMongoDatabase db)
    {
        _readCollection = db.GetCollection<User>("user_read");
    }

    public Task<User?> GetByIdAsync(ObjectId id)
    {
        return _readCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public Task<List<User>> FindAsync(string? username, string? email, int pageSize, int pageNumber)
    {
        var filter = Builders<User>.Filter.Empty;
        if (!string.IsNullOrEmpty(username))
            filter &= Builders<User>.Filter.Eq(u => u.Username, username);
        if (!string.IsNullOrEmpty(email))
            filter &= Builders<User>.Filter.Eq(u => u.Email, email);

        if (pageSize <= 0) pageSize = 10;
        if (pageNumber <= 0) pageNumber = 1;

        return _readCollection.Find(filter)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }
}
