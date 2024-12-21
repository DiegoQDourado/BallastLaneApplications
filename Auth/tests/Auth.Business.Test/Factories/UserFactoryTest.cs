using Auth.Business.Entities;
using Auth.Business.Factories;
using Auth.Business.Factories.Impl;
using Auth.Business.Models;
using NUnit.Framework;

namespace Auth.Business.Test.Factories
{
    public class UserFactoryTest
    {
        private IUserFactory _userFactory;

        [SetUp]
        public void Setup()
        {
            _userFactory = new UserFactory();
        }

        [Test]
        public void From_UserModelToUserEntity_ReturnsCorrectUserEntity()
        {
            // Arrange
            var userModel = new UserModel
            {
                Id = Guid.NewGuid(),
                UserName = "TestUser",
                PasswordHash = "hashedpassword123",
                Roles = "Admin,User"
            };

            // Act
            var result = _userFactory.From(userModel);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(userModel.Id));
            Assert.That(result.UserName, Is.EqualTo(userModel.UserName));
            Assert.That(result.Roles, Is.EqualTo(userModel.Roles));
            Assert.That(result.PasswordHash, Is.EqualTo(userModel.PasswordHash));
        }

        [Test]
        public void From_UserEntityToUserModel_ReturnsCorrectUserModel()
        {
            // Arrange
            var userEntity = new UserEntity
            {
                Id = Guid.NewGuid(),
                UserName = "EntityUser",
                Roles = "User"
            };

            userEntity.SetPasswordHash("entityhashedpassword456");

            // Act
            var result = _userFactory.From(userEntity);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(userEntity.Id));
            Assert.That(result.UserName, Is.EqualTo(userEntity.UserName));
            Assert.That(result.Roles, Is.EqualTo(userEntity.Roles));
            Assert.That(result.PasswordHash, Is.EqualTo(userEntity.PasswordHash));
        }

        [Test]
        public void From_ListOfUserEntities_ReturnsListOfUserModels()
        {
            // Arrange
            var userEnitityId1 = Guid.NewGuid();
            var userEnitityId2 = Guid.NewGuid();
            var users = new List<UserEntity>
            {
                new() { Id = userEnitityId1, UserName = "User1", Roles = "Admin,User" },
                new () { Id = userEnitityId2, UserName = "User2", Roles = "User" },
            };
            users[0].SetPasswordHash("password1");
            users[1].SetPasswordHash("password2");

            // Act
            var result = _userFactory.From(users).ToList();

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));

            Assert.That(result[0].Id, Is.EqualTo(userEnitityId1));
            Assert.That(result[0].UserName, Is.EqualTo("User1"));
            Assert.That(result[0].PasswordHash, Is.EqualTo("password1"));
            Assert.That(result[0].Roles, Is.EqualTo("Admin,User"));

            Assert.That(result[1].Id, Is.EqualTo(userEnitityId2));
            Assert.That(result[1].UserName, Is.EqualTo("User2"));
            Assert.That(result[1].PasswordHash, Is.EqualTo("password2"));
            Assert.That(result[1].Roles, Is.EqualTo("User"));
        }
    }

}


