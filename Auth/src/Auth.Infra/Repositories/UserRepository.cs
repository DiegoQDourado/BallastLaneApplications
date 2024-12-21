using Auth.Business.Entities;
using Auth.Business.Repositories;
using Auth.Infra.Configs;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Auth.Infra.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(DatabaseConfig databaseConfig)
        {
            _connectionString = databaseConfig.ConnectionString;
        }

        public async Task<UserEntity?> GetByAsync(string userName)
        {
            UserEntity user = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT Top 1 Id, UserName, PasswordHash, Roles FROM Users WHERE UserName = @UserName", connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserName", SqlDbType.NVarChar) { Value = userName });

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new UserEntity
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                Roles = reader.GetString(reader.GetOrdinal("Roles"))
                            };

                            user.SetPasswordHash(reader.GetString(reader.GetOrdinal("PasswordHash")));
                        }
                    }
                }
            }

            return user;
        }

        public async Task<UserEntity?> GetByOrAsync(Guid id, string userName)
        {
            UserEntity user = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT Top 1 Id, UserName, PasswordHash, Roles FROM Users WHERE UserName = @UserName OR Id = @Id", connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserName", SqlDbType.NVarChar) { Value = userName });
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new UserEntity
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                Roles = reader.GetString(reader.GetOrdinal("Roles"))
                            };

                            user.SetPasswordHash(reader.GetString(reader.GetOrdinal("PasswordHash")));
                        }
                    }
                }
            }

            return user;
        }

        public async Task AddAsync(UserEntity user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(
                    "INSERT INTO Users (Id, Username, PasswordHash, Roles) VALUES (@Id, @Username, @PasswordHash, @Roles)", connection))
                {
                    command.Parameters.AddRange(
                        [
                        new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = user.Id },
                        new SqlParameter("@Username", SqlDbType.NVarChar) { Value = user.UserName },
                        new SqlParameter("@PasswordHash", SqlDbType.NVarChar) { Value = user.PasswordHash },
                        new SqlParameter("@Roles", SqlDbType.VarChar) { Value = user.Roles }
                ]);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }


}
