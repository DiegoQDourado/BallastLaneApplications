using Auth.Api.Handlers;
using Auth.Business.Handlers;

namespace Auth.Api.Extensions
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddSecurity(this IServiceCollection services) =>
            services
                .AddSingleton<IJwtHandler, JwtHandler>()
                .AddSingleton<IPasswordHandler, PasswordHandler>();
    }
}
