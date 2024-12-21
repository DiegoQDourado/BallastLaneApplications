using Microsoft.Extensions.DependencyInjection;
using Product.Business.Factories;
using Product.Business.Factories.Impl;
using Product.Business.Services;
using Product.Business.Services.Impl;
using SharedKernel.Notifications;
using SharedKernel.Notifications.Impl;

namespace Product.Business.Extensions
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddBusiness(this IServiceCollection services) =>
            services
               .AddScoped<IProductFactory, ProductFactory>()
               .AddScoped<INotification, Notification>()
               .AddScoped<IProductService, ProductService>();
    }
}
