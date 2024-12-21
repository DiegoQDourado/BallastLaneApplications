using Auth.Business.Entities;
using Auth.Business.Repositories;
using Auth.Infra.Configs;
using Auth.Infra.Repositories;
using Dapper;
using Microsoft.Data.Sqlite;
using NUnit.Framework;

namespace Auth.Infra.Test.Repositories
{
    [TestFixture]
    public class UserRepositoryIntegrationTests
    {
        private SqliteConnection _connection;
        private IUserRepository _userRepository;

        [SetUp]
        public void Setup()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // Cria a tabela em memória
            _connection.Execute(@"
                CREATE TABLE Users (
                    Id UNIQUEIDENTIFIER PRIMARY KEY,
                    UserName NVARCHAR(100),
                    PasswordHash NVARCHAR(255),
                    Roles VARCHAR(100)
                )");

            var databaseConfig = new DatabaseConfig { ConnectionString = _connection.ConnectionString };
            _userRepository = new UserRepository(databaseConfig);
        }

        [TearDown]
        public void TearDown()
        {
            _connection.Dispose();
        }

        [Test]
        public async Task AddAsync_ShouldInsertUserIntoDatabase()
        {
            // Arrange
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Roles = "Admin"
            };
            user.SetPasswordHash("hashedPassword");

            // Act
            await _userRepository.AddAsync(user);

            // Assert
            var result = await _connection.QuerySingleOrDefaultAsync<UserEntity>(
                "SELECT * FROM Users WHERE UserName = @UserName", new { user.UserName });
            
            Assert.That(result, Is.Not.Null);
            Assert.That(user.UserName, Is.EqualTo(result.UserName));
            Assert.That("hashedPassword", Is.EqualTo(result.PasswordHash));
            Assert.That("Admin", Is.EqualTo(result.Roles));              
        }

        [Test]
        public async Task GetByAsync_WhenUserExists_ReturnsUserEntity()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userName = "existinguser";
            await _connection.ExecuteAsync(
                "INSERT INTO Users (Id, UserName, PasswordHash, Roles) VALUES (@Id, @UserName, @PasswordHash, @Roles)",
                new { Id = userId, UserName = userName, PasswordHash = "hashedPassword", Roles = "User" });

            // Act
            var result = await _userRepository.GetByAsync(userName);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(userId, Is.EqualTo(result.Id));
            Assert.That(userName, Is.EqualTo(result.UserName));            
            Assert.That("hashedPassword", Is.EqualTo(result.PasswordHash));
            Assert.That("User", Is.EqualTo(result.Roles));
        }

        [Test]
        public async Task GetByAsync_WhenUserDoesNotExist_ReturnsNull()
        {
            // Act
            var result = await _userRepository.GetByAsync("nonexistentuser");

            // Assert
            Assert.That(result, Is.Not.Null);
        }
    }
}
