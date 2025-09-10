using ApiReadUsers.Business;
using ApiReadUsers.Repositories;
using MongoDB.Bson;

namespace ApiReadUsers.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<User?> GetUserAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;

        return await _repository.GetByIdAsync(objectId);
    }

    public async Task<(List<User> Users, int PageSize, int PageNumber)> GetUsersAsync(string? username, string? email, int pageSize, int pageNumber)
    {
        if (pageSize <= 0) pageSize = 10;
        if (pageNumber <= 0) pageNumber = 1;

        var users = await _repository.FindAsync(username, email, pageSize, pageNumber);
        return (users, pageSize, pageNumber);
    }
}
