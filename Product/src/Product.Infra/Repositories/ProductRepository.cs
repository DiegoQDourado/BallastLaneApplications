using Microsoft.Data.SqlClient;
using Product.Business.Entities;
using Product.Business.Repositories;
using Product.Infra.Configs;
using System.Data;

namespace Product.Infra.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(DatabaseConfig databaseConfig)
        {
            _connectionString = databaseConfig.ConnectionString;
        }

        public async Task<IEnumerable<ProductEntity>> GetAllAsync()
        {
            var products = new List<ProductEntity>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT Id, Name, Description, Price FROM Products", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(new ProductEntity
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price"))
                            });
                        }
                    }
                }
            }

            return products;
        }

        public async Task<ProductEntity> GetByAsync(Guid id)
        {
            ProductEntity product = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT Id, Name, Description, Price FROM Products WHERE Id = @Id", connection))
                {
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            product = new ProductEntity
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price"))
                            };
                        }
                    }
                }
            }

            return product;
        }

        public async Task<ProductEntity> GetByOrAsync(Guid id, string name)
        {
            ProductEntity product = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT Id, Name, Description, Price FROM Products WHERE Name = @Name OR Id = @Id", connection))
                {
                    command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar) { Value = name });
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            product = new ProductEntity
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price"))
                            };
                        }
                    }
                }
            }

            return product;
        }

        public async Task AddAsync(ProductEntity product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(
                    "INSERT INTO Products (Id, Name, Description, Price) VALUES (@Id, @Name, @Description, @Price)", connection))
                {
                    command.Parameters.AddRange(new[]
                    {
                    new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = product.Id },
                    new SqlParameter("@Name", SqlDbType.NVarChar) { Value = product.Name },
                    new SqlParameter("@Description", SqlDbType.NVarChar) { Value = product.Description },
                    new SqlParameter("@Price", SqlDbType.Decimal) { Value = product.Price }
                });

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateAsync(ProductEntity product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(
                    "UPDATE Products SET Name = @Name, Description = @Description, Price = @Price WHERE Id = @Id", connection))
                {
                    command.Parameters.AddRange(new[]
                    {
                    new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = product.Id },
                    new SqlParameter("@Name", SqlDbType.NVarChar) { Value = product.Name },
                    new SqlParameter("@Description", SqlDbType.NVarChar) { Value = product.Description },
                    new SqlParameter("@Price", SqlDbType.Decimal) { Value = product.Price }
                });

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("DELETE FROM Products WHERE Id = @Id", connection))
                {
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });
                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }
    }

}
