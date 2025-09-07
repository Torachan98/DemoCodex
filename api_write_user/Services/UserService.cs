using ApiWriteUser.Business;
using ApiWriteUser.Repositories;
using MongoDB.Bson;

namespace ApiWriteUser.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<User> CreateAsync(UserCreate request)
    {
        var user = new User
        {
            Id = ObjectId.GenerateNewId(),
            Username = request.Username,
            Email = request.Email,
            IsActive = true,
            IsSync = false,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow
        };

        await _repository.InsertAsync(user);
        return user;
    }

    public Task<User?> UpdateAsync(string id, UserCreate request)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return Task.FromResult<User?>(null);

        return _repository.UpdateAsync(objectId, request);
    }

    public Task<bool> DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return Task.FromResult(false);

        return _repository.DeleteAsync(objectId);
    }
}
