using FluentValidation;

namespace Auth.Business.Entities.Validations
{
    public class UserValidator : AbstractValidator<UserEntity>
    {

        public UserValidator()
        {
            RuleFor(a => a.Id)
                .NotEmpty()
                .WithMessage("User Id could not be empty.");

            RuleFor(a => a.UserName)
                .NotEmpty()
                .WithMessage("User UserName could not be empty.");

            RuleFor(a => a.PasswordHash)
               .NotEmpty()
               .WithMessage("User PasswordHash could not be empty.");

            RuleFor(a => a.Roles)
              .NotEmpty()
              .WithMessage("User Roles could not be empty.")
              .Matches(@"^(Admin|User)(,(Admin|User))*$")
              .WithMessage("Roles must only contain 'Admin' and 'User' separated by commas.");
        }
    }
}
