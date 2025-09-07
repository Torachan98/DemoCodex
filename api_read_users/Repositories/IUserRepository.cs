using ApiReadUsers.Business;
using MongoDB.Bson;

namespace ApiReadUsers.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(ObjectId id);
    Task<List<User>> FindAsync(string? username, string? email, int pageSize, int pageNumber);
}
