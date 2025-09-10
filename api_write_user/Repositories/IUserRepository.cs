using ApiWriteUser.Business;
using MongoDB.Bson;

namespace ApiWriteUser.Repositories;

public interface IUserRepository
{
    Task InsertAsync(User user);
    Task<User?> UpdateAsync(ObjectId id, UserCreate request);
    Task<bool> DeleteAsync(ObjectId id);
}
