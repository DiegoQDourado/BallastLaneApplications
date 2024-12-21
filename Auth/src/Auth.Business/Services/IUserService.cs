using Auth.Business.Models;

namespace Auth.Business.Services
{
    public interface IUserService
    {
        Task CreateAsync(UserModel user);
        Task<string> LoginAsync(string userName, string password);
    }
}
