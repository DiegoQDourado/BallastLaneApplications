using Auth.Api.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Auth.Api.Examples.Requests
{
    public class UserLoginRequestExample : IExamplesProvider<UserLogin>
    {
        public UserLogin GetExamples() => new(UserName: "UserName Example", Password: "Password Example");

    }
}
