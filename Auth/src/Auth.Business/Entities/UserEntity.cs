using Auth.Business.Entities.Validations;
using FluentValidation.Results;

namespace Auth.Business.Entities
{
    public class UserEntity : BaseEntity
    {
        public Guid Id { get; init; }
        public string UserName { get; init; }
        public string PasswordHash { get; protected set; }
        public string Roles { get; init; }

        public void SetPasswordHash(string passowordHash) =>
            PasswordHash = passowordHash;

        protected override ValidationResult GetValidation() =>
            new UserValidator().Validate(this);
    }
}
