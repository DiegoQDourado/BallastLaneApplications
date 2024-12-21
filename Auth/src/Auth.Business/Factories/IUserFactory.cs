using Auth.Business.Entities;
using Auth.Business.Models;

namespace Auth.Business.Factories
{
    public interface IUserFactory
    {
        UserEntity From(UserModel user);
        UserModel From(UserEntity user);
        IEnumerable<UserModel> From(IEnumerable<UserEntity> users);
    }
}
