using NUnit.Framework;
using Product.Business.Entities;
using Product.Business.Factories;
using Product.Business.Factories.Impl;
using Product.Business.Models;

namespace Product.Business.Test.Factories
{
    public class ProductFactoryTest
    {

        private IProductFactory _productFactory;

        [SetUp]
        public void Setup()
        {
            _productFactory = new ProductFactory();
        }

        [Test]
        public void From_ProductModel_CorrectlyMapsToProductEntity()
        {
            // Arrange 
            var productModel = new ProductModel
            {
                Id = Guid.NewGuid(),
                Name = "Test Product",
                Description = "This is a test product.",
                Price = 10m
            };

            // Act
            var productEntity = _productFactory.From(productModel);

            // Assert
            Assert.That(productEntity, Is.Not.Null);
            Assert.That(productModel.Id, Is.EqualTo(productEntity.Id));
            Assert.That(productModel.Name, Is.EqualTo(productEntity.Name));
            Assert.That(productModel.Description, Is.EqualTo(productEntity.Description));
            Assert.That(productModel.Price, Is.EqualTo(productEntity.Price));
        }

        [Test]
        public void From_ProductEntity_CorrectlyMapsToProductModel()
        {
            // Arrange   
            var productEntity = new ProductEntity
            {
                Id = Guid.NewGuid(),
                Name = "Test Product",
                Description = "This is a test product.",
                Price = 10m
            };

            // Act
            var productModel = _productFactory.From(productEntity);

            // Assert
            Assert.That(productModel, Is.Not.Null);
            Assert.That(productEntity.Id, Is.EqualTo(productModel.Id));
            Assert.That(productEntity.Name, Is.EqualTo(productModel.Name));
            Assert.That(productEntity.Description, Is.EqualTo(productModel.Description));
            Assert.That(productEntity.Price, Is.EqualTo(productModel.Price));

        }

        [Test]
        public void From_ProductEntityList_CorrectlyMapsToProductModelList()
        {
            // Arrange 
            var userEnitityId1 = Guid.NewGuid();
            var userEnitityId2 = Guid.NewGuid();
            var productEntities = new List<ProductEntity>
            {
                new() { Id = userEnitityId1, Name = "Product 1", Description = "Desc 1", Price = 10.0m },
                new () { Id = userEnitityId2, Name = "Product 2", Description = "Desc 2", Price = 20.0m }
            };

            // Act
            var productModels = _productFactory.From(productEntities).ToList();

            // Assert
            Assert.That(productModels, Is.Not.Null);
            Assert.That(productModels, Has.Count.EqualTo(2));
            var productModelList = productModels.ToList();

            Assert.That(productEntities[0].Id, Is.EqualTo(productModelList[0].Id));
            Assert.That(productEntities[0].Name, Is.EqualTo(productModelList[0].Name));
            Assert.That(productEntities[0].Description, Is.EqualTo(productModelList[0].Description));
            Assert.That(productEntities[0].Price, Is.EqualTo(productModelList[0].Price));
            Assert.That(productEntities[1].Id, Is.EqualTo(productModelList[1].Id));
            Assert.That(productEntities[1].Name, Is.EqualTo(productModelList[1].Name));
            Assert.That(productEntities[1].Description, Is.EqualTo(productModelList[1].Description));
            Assert.That(productEntities[1].Price, Is.EqualTo(productModelList[1].Price));
        }
    }
}
