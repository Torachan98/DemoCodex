using ApiWriteUser.Business;

namespace ApiWriteUser.Services;

public interface IUserService
{
    Task<User> CreateAsync(UserCreate request);
    Task<User?> UpdateAsync(string id, UserCreate request);
    Task<bool> DeleteAsync(string id);
}
