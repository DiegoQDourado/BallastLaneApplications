﻿using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product.Business.Repositories;
using Product.Infra.Configs;
using Product.Infra.Repositories;

namespace Product.Infra.Extensions
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration) =>
            services
                .AddDatabaseConfig(configuration)
               .AddScoped<IProductRepository, ProductRepository>();


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
