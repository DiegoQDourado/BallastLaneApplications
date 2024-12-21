using Auth.Business.Entities;
using Auth.Business.Models;

namespace Auth.Business.Factories.Impl
{
    internal class UserFactory : IUserFactory
    {
        public UserEntity From(UserModel user)
        {
            var userEntity = new UserEntity()
            {
                Id = user.Id,
                UserName = user.UserName,
                Roles = user.Roles,
            };

            userEntity.SetPasswordHash(user.PasswordHash);
            return userEntity;
        }

        public UserModel From(UserEntity user) =>
           new()
           {
               Id = user.Id,
               UserName = user.UserName,
               PasswordHash = user.PasswordHash,
               Roles = user.Roles,
           };

        public IEnumerable<UserModel> From(IEnumerable<UserEntity> users) =>
            users.Select(ue => From(ue));

    }

}
