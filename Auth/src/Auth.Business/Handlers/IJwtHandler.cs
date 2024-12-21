using Auth.Business.Models;

namespace Auth.Api.Handlers
{
    public interface IJwtHandler
    {
        string GenerateToken(UserModel user);
    }
}