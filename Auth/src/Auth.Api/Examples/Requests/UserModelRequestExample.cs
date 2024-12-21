using Auth.Business.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Auth.Api.Examples.Requests
{
    public class UserModelRequestExample : IExamplesProvider<UserModel>
    {
        public UserModel GetExamples() => new()
        {

            Id = Guid.NewGuid(),
            UserName = "UserName Example",
            Password = "Password Example",
            Roles = "Admin,User"

        };
    }
}
