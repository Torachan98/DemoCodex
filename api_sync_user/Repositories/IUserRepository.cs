using ApiSyncUser.Business;
using MongoDB.Bson;

namespace ApiSyncUser.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetUnsyncedAsync();
    Task InsertReadAsync(IEnumerable<User> users);
    Task MarkSyncedAsync(IEnumerable<ObjectId> ids);
}
