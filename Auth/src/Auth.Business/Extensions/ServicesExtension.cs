using Auth.Business.Factories;
using Auth.Business.Factories.Impl;
using Auth.Business.Services;
using Auth.Business.Services.Impl;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Notifications;
using SharedKernel.Notifications.Impl;

namespace Auth.Business.Extensions
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddBusiness(this IServiceCollection services) =>
            services
               .AddScoped<IUserFactory, UserFactory>()
               .AddScoped<INotification, Notification>()
               .AddScoped<IUserService, UserService>();
    }
}
