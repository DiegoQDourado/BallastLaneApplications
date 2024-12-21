using Auth.Business.Repositories;
using Auth.Infra.Configs;
using Auth.Infra.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infra.Extensions
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration) =>
            services
                .AddDatabaseConfig(configuration)
               .AddScoped<IUserRepository, UserRepository>();


        private static IServiceCollection AddDatabaseConfig(
           this IServiceCollection services,
           IConfiguration configuration) =>
           services.AddSingleton(_ =>
           {
               var connectionString = configuration.GetConnectionString("DefaultConnection");
               var sqlConnection = new SqlConnectionStringBuilder(connectionString)
               {
                   PoolBlockingPeriod = PoolBlockingPeriod.NeverBlock,
                   MinPoolSize = 50,
               };

               return new DatabaseConfig()
               {
                   ConnectionString = sqlConnection.ConnectionString,
               };
           });
    }
}
