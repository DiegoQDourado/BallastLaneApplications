using Auth.Business.Entities;

namespace Auth.Business.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(UserEntity user);
        Task<UserEntity?> GetByAsync(string userName);
        Task<UserEntity?> GetByOrAsync(Guid id, string userName);
    }
}
