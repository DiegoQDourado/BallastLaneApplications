using Auth.Api.Handlers;
using Auth.Business.Entities;
using Auth.Business.Factories;
using Auth.Business.Handlers;
using Auth.Business.Models;
using Auth.Business.Repositories;
using Auth.Business.Services.Impl;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SharedKernel.Enums;
using SharedKernel.Notifications;

namespace Auth.Business.Test.Services
{

    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IUserFactory> _userFactoryMock;
        private Mock<INotification> _notificationMock;
        private Mock<ILogger<UserService>> _loggerMock;
        private Mock<IPasswordHandler> _passwordHandlerMock;
        private Mock<IJwtHandler> _jwtHandlerMock;

        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userFactoryMock = new Mock<IUserFactory>();
            _notificationMock = new Mock<INotification>();
            _loggerMock = new Mock<ILogger<UserService>>();
            _passwordHandlerMock = new Mock<IPasswordHandler>();
            _jwtHandlerMock = new Mock<IJwtHandler>();

            _userService = new UserService(
                _userRepositoryMock.Object,
                _userFactoryMock.Object,
                _loggerMock.Object,
                _notificationMock.Object,
                _passwordHandlerMock.Object,
                _jwtHandlerMock.Object
            );
        }

        [Test]
        public async Task CreateAsync_UserAlreadyExists_AddsNotification()
        {
            // Arrange
            var id = Guid.NewGuid();
            var userModel = new UserModel { Id = id, UserName = "existingUser" };
            var existingUserEntity = new UserEntity { Id = id, UserName = "existingUser" };

            _userRepositoryMock
                .Setup(repo => repo.GetByOrAsync(userModel.Id, userModel.UserName))
                .ReturnsAsync(existingUserEntity);

            // Act
            await _userService.CreateAsync(userModel);

            // Assert
            _notificationMock.Verify(n => n.Add($"User {existingUserEntity.UserName} or Id {existingUserEntity.Id} already exists.", NotificationType.Expected), Times.Once);
        }

        [Test]
        public async Task CreateAsync_PasswordIsNull_AddsNotification()
        {
            // Arrange
            var userModel = new UserModel { UserName = "newUser", Password = null };

            _userRepositoryMock
                .Setup(repo => repo.GetByAsync(userModel.UserName))
                .ReturnsAsync((UserEntity)null);

            // Act
            await _userService.CreateAsync(userModel);

            // Assert
            _notificationMock.Verify(n => n.Add("Password is required.", NotificationType.Expected), Times.Once);
        }

        [Test]
        public async Task CreateAsync_ValidUser_CallsAddAsync()
        {
            // Arrange
            var userModel = new UserModel
            {
                Id = Guid.NewGuid(),
                UserName = "newUser",
                Password = "password123",
                Roles = "Admin"
            };

            var hashedPassword = "hashedPassword";
            var userEntity = new UserEntity
            {
                Id = Guid.NewGuid(),
                UserName = "newUser",
                Roles = "Admin"
            };
            userEntity.SetPasswordHash(hashedPassword);

            _userRepositoryMock
                .Setup(repo => repo.GetByAsync(userModel.UserName))
                .ReturnsAsync((UserEntity)null);

            _userFactoryMock
                .Setup(factory => factory.From(userModel))
                .Returns(userEntity);

            _passwordHandlerMock
                .Setup(handler => handler.HashPassword(userModel.Password))
                .Returns(hashedPassword);

            _userRepositoryMock
                .Setup(repo => repo.AddAsync(userEntity))
                .Returns(Task.CompletedTask);

            // Act
            await _userService.CreateAsync(userModel);

            // Assert
            _userRepositoryMock.Verify(repo => repo.AddAsync(userEntity), Times.Once);
        }

        [Test]
        public async Task TryCreateAsync_InvalidUser_CallsAddAsync()
        {
            // Arrange
            var userModel = new UserModel
            {
                UserName = "newUser",
                Password = "password123"
            };

            var userEntity = new UserEntity
            {
                UserName = "newUser",
            };

            var hashedPassword = "hashedPassword";
            userEntity.SetPasswordHash(hashedPassword);

            _userRepositoryMock
                .Setup(repo => repo.GetByAsync(userModel.UserName))
                .ReturnsAsync((UserEntity)null);

            _userFactoryMock
                .Setup(factory => factory.From(userModel))
                .Returns(userEntity);

            _passwordHandlerMock
                .Setup(handler => handler.HashPassword(userModel.Password))
                .Returns("hashedPassword");

            _userRepositoryMock
                .Setup(repo => repo.AddAsync(userEntity))
                .Returns(Task.CompletedTask);

            // Act
            await _userService.CreateAsync(userModel);

            // Assert
            _userRepositoryMock.Verify(repo => repo.AddAsync(userEntity), Times.Never);
            _notificationMock.Verify(repo => repo.Add(It.IsAny<IEnumerable<string>>()), Times.Once);
        }

        [Test]
        public async Task LoginAsync_UserNotFound_AddsNotificationAndLogsInfo()
        {
            // Arrange
            var userName = "nonExistingUser";
            var password = "password123";

            _userRepositoryMock
                .Setup(repo => repo.GetByAsync(userName))
                .ReturnsAsync((UserEntity)null);

            // Act
            var token = await _userService.LoginAsync(userName, password);

            // Assert
            Assert.That(token, Is.EqualTo(string.Empty));
            _notificationMock.Verify(n => n.Add("Invalid UserName/Password.", NotificationType.Expected), Times.Once);
            _loggerMock.Verify(
                log => log.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Login attempt for {userName} not found.")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task LoginAsync_InvalidPassword_AddsNotificationAndLogsInfo()
        {
            // Arrange
            var userName = "existingUser";
            var password = "wrongPassword";
            var userEntity = new UserEntity { UserName = userName };
            userEntity.SetPasswordHash("hashedPassword");

            _userRepositoryMock
                .Setup(repo => repo.GetByAsync(userName))
                .ReturnsAsync(userEntity);

            _passwordHandlerMock
                .Setup(handler => handler.VerifyPassword(password, userEntity.PasswordHash))
                .Returns(false);

            // Act
            var token = await _userService.LoginAsync(userName, password);

            // Assert
            Assert.That(token, Is.EqualTo(string.Empty));
            _notificationMock.Verify(n => n.Add("Invalid UserName/Password.", NotificationType.Expected), Times.AtLeastOnce);
            _loggerMock.Verify(
                log => log.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Login attempt for {userName} with not valid password.")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);
        }

        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var userName = "validUser";
            var password = "password123";
            var userEntity = new UserEntity { UserName = userName };
            userEntity.SetPasswordHash("hashedPassword");
            var userModel = new UserModel { UserName = userName };
            var token = "validToken";

            _userRepositoryMock
                .Setup(repo => repo.GetByAsync(userName))
                .ReturnsAsync(userEntity);

            _passwordHandlerMock
                .Setup(handler => handler.VerifyPassword(password, userEntity.PasswordHash))
                .Returns(true);

            _userFactoryMock
                .Setup(factory => factory.From(userEntity))
                .Returns(userModel);

            _jwtHandlerMock
                .Setup(handler => handler.GenerateToken(userModel))
                .Returns(token);

            // Act
            var result = await _userService.LoginAsync(userName, password);

            // Assert
            Assert.That(result, Is.EqualTo(token));
        }
    }
}
