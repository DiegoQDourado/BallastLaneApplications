using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Product.Business.Entities;
using Product.Business.Factories;
using Product.Business.Models;
using Product.Business.Repositories;
using Product.Business.Services.Impl;
using SharedKernel.Enums;
using SharedKernel.Notifications;

namespace Product.Business.Test.Services
{
    [TestFixture]
    public class ProductServiceTests
    {
        private Mock<IProductRepository> _mockProductRepository;
        private Mock<IProductFactory> _mockProductFactory;
        private Mock<INotification> _mockNotification;
        private Mock<ILogger<ProductService>> _mockLogger;
        private ProductService _productService;

        [SetUp]
        public void Setup()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockProductFactory = new Mock<IProductFactory>();
            _mockNotification = new Mock<INotification>();
            _mockLogger = new Mock<ILogger<ProductService>>();

            _productService = new ProductService(
                _mockProductRepository.Object,
                _mockProductFactory.Object,
                _mockLogger.Object,
                _mockNotification.Object
            );
        }

        [Test]
        public async Task CreateAsync_ShouldAddProduct_WhenProductDoesNotExist()
        {
            // Arrange
            var productModel = new ProductModel { Id = Guid.NewGuid(), Name = "New Product", Description = "Description", Price = 10 };
            var productEnity = new ProductEntity { Id = productModel.Id, Name = productModel.Name, Description = productModel.Description, Price = productModel.Price };
            _mockProductRepository.Setup(repo => repo.GetByOrAsync(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync((ProductEntity)null);
            _mockProductFactory.Setup(factory => factory.From(It.IsAny<ProductModel>())).Returns(productEnity);
            _mockProductRepository.Setup(repo => repo.AddAsync(It.IsAny<ProductEntity>())).Returns(Task.CompletedTask);

            // Act
            await _productService.CreateAsync(productModel);

            // Assert
            _mockProductRepository.Verify(repo => repo.AddAsync(It.IsAny<ProductEntity>()), Times.Once);
            _mockNotification.Verify(notification => notification.Add(It.IsAny<string>(), NotificationType.Expected), Times.Never);
        }

        [Test]
        public async Task CreateAsync_ShouldNotify_WhenProductAlreadyExists()
        {
            // Arrange
            var productModel = new ProductModel { Id = Guid.NewGuid(), Name = "Existing Product" };
            _mockProductRepository.Setup(repo => repo.GetByOrAsync(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(new ProductEntity());

            // Act
            await _productService.CreateAsync(productModel);

            // Assert
            _mockNotification.Verify(notification => notification.Add(It.Is<string>(s => s.Contains("already exists")), NotificationType.Expected), Times.Once);
            _mockProductRepository.Verify(repo => repo.AddAsync(It.IsAny<ProductEntity>()), Times.Never);
        }

        [Test]
        public async Task CreateAsync_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var productModel = new ProductModel { Id = Guid.NewGuid(), Name = "New Product", Description = "Description", Price = 10 };
            _mockProductRepository.Setup(repo => repo.GetByOrAsync(It.IsAny<Guid>(), It.IsAny<string>())).ThrowsAsync(new Exception("Database error"));

            // Act
            await _productService.CreateAsync(productModel);

            // Assert
            _mockLogger.Verify(
                log => log.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Failed to add product {productModel.Name} Error: Database error.")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);

            _mockNotification.Verify(notification => notification.Add(It.IsAny<string>(), NotificationType.Unexpected), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldNotify_WhenProductNotFound()
        {
            // Arrange
            _mockProductRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            // Act
            await _productService.DeleteAsync(Guid.NewGuid());

            // Assert
            _mockNotification.Verify(notification => notification.Add(It.Is<string>(s => s.Contains("not found")), NotificationType.NotFound), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldNotNotify_WhenProductDeletedSuccessfully()
        {
            // Arrange
            _mockProductRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            // Act
            await _productService.DeleteAsync(Guid.NewGuid());

            // Assert
            _mockNotification.Verify(notification => notification.Add(It.IsAny<string>(), NotificationType.Expected), Times.Never);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnProducts()
        {
            // Arrange
            var productEntities = new List<ProductEntity> { new() { Id = Guid.NewGuid(), Name = "Product 1" } };
            _mockProductRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(productEntities);
            _mockProductFactory.Setup(factory => factory.From(It.IsAny<IEnumerable<ProductEntity>>())).Returns([]);

            // Act
            var result = await _productService.GetAllAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<IEnumerable<ProductModel>>());
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productEntity = new ProductEntity { Id = productId, Name = "Product 1" };
            _mockProductRepository.Setup(repo => repo.GetByAsync(productId)).ReturnsAsync(productEntity);
            _mockProductFactory.Setup(factory => factory.From(productEntity)).Returns(new ProductModel());

            // Act
            var result = await _productService.GetByIdAsync(productId);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetByIdAsync_ShouldNotify_WhenProductNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockProductRepository.Setup(repo => repo.GetByAsync(productId)).ReturnsAsync((ProductEntity)null);

            // Act
            var result = await _productService.GetByIdAsync(productId);

            // Assert
            Assert.That(result, Is.Null);
            _mockNotification.Verify(notification => notification.Add(It.Is<string>(s => s.Contains("not found")), NotificationType.NotFound), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateProduct_WhenValid()
        {
            // Arrange
            var productModel = new ProductModel { Id = Guid.NewGuid(), Name = "Updated Product", Description = "Description", Price = 10 };
            var productEntity = new ProductEntity { Id = productModel.Id, Name = "Old Product", Description = "Description", Price = 10 };
            _mockProductRepository.Setup(repo => repo.GetByAsync(productModel.Id)).ReturnsAsync(productEntity);
            _mockProductFactory.Setup(factory => factory.From(It.IsAny<ProductModel>())).Returns(productEntity);
            _mockProductRepository.Setup(repo => repo.UpdateAsync(It.IsAny<ProductEntity>())).Returns(Task.CompletedTask);

            // Act
            await _productService.UpdateAsync(productModel);

            // Assert
            _mockProductRepository.Verify(repo => repo.UpdateAsync(It.IsAny<ProductEntity>()), Times.Once);
            _mockNotification.Verify(notification => notification.Add(It.IsAny<string>(), NotificationType.Expected), Times.Never);
        }

        [Test]
        public async Task UpdateAsync_ShouldNotify_WhenProductNotFound()
        {
            // Arrange
            var productModel = new ProductModel { Id = Guid.NewGuid(), Name = "Updated Product" };
            _mockProductRepository.Setup(repo => repo.GetByAsync(productModel.Id)).ReturnsAsync((ProductEntity)null);

            // Act
            await _productService.UpdateAsync(productModel);

            // Assert
            _mockNotification.Verify(notification => notification.Add(It.Is<string>(s => s.Contains("Product not found")), NotificationType.NotFound), Times.Once);
        }
    }
}

