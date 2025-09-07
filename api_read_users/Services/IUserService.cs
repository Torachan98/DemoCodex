using ApiReadUsers.Business;

namespace ApiReadUsers.Services;

public interface IUserService
{
    Task<User?> GetUserAsync(string id);
    Task<(List<User> Users, int PageSize, int PageNumber)> GetUsersAsync(string? username, string? email, int pageSize, int pageNumber);
}
