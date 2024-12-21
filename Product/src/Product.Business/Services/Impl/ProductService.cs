using Microsoft.Extensions.Logging;
using SharedKernel.Enums;
using Product.Business.Factories;
using Product.Business.Models;
using SharedKernel.Notifications;
using Product.Business.Repositories;
using System.Threading.Tasks;
using System;


namespace Product.Business.Services.Impl
{
    internal class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductFactory _productFactory;
        private readonly INotification _notification;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, IProductFactory productFactory, ILogger<ProductService> logger, INotification notification)
        {
            _productRepository = productRepository;
            _productFactory = productFactory;
            _logger = logger;
            _notification = notification;
        }

        public async Task CreateAsync(ProductModel product)
        {
            try
            {
                var productEntity = await _productRepository.GetByOrAsync(product.Id, product.Name);
                if (productEntity is { })
                {
                    _notification.Add($"Product {product.Name} or Id {product.Id} already exists.");
                    return;
                }

                productEntity = _productFactory.From(product);
                var validation = productEntity.Validate();

                if (validation.Any())
                {
                    _notification.Add(validation);
                    return;
                }

                await _productRepository.AddAsync(productEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to add product {Name} Error: {Message}.", product.Name, ex.Message);
                _notification.Add($"Failed to add product {product.Name}.", NotificationType.Unexpected);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var isDeleted = await _productRepository.DeleteAsync(id);

                if(!isDeleted)
                {
                    _notification.Add($"Product with ID {id} not found.", NotificationType.NotFound);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete product {Id} Error: {Message}.", id, ex.Message);
                _notification.Add($"Failed to delete product {id}.", NotificationType.Unexpected);
            }
        }

        public async Task<IEnumerable<ProductModel>> GetAllAsync()
        {
            try
            {
                var productsEntity = await _productRepository.GetAllAsync();
                return _productFactory.From(productsEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get all products Error: {Message}.", ex.Message);
                _notification.Add($"Failed to get all products.", NotificationType.Unexpected);
            }

            return [];
        }

        public async Task<ProductModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var productEntity = await _productRepository.GetByAsync(id);
                if (productEntity is null)
                {
                    _notification.Add($"Product with ID {id} not found.", NotificationType.NotFound);
                }
                return productEntity is { } ? _productFactory.From(productEntity) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get product {Id} Error: {Message}.", id, ex.Message);
                _notification.Add($"Failed to get product {id}.", NotificationType.Unexpected);
            }

            return null;
        }

        public async Task UpdateAsync(ProductModel product)
        {
            try
            {
                var productEntity = await _productRepository.GetByAsync(product.Id);
                if (productEntity is { })
                {
                    productEntity = _productFactory.From(product);
                    var validation = productEntity.Validate();

                    if (validation.Any())
                    {
                        _notification.Add(validation);
                        return;
                    }

                    await _productRepository.UpdateAsync(productEntity);
                    return;
                }

                _notification.Add($"Product not found {product.Id}", NotificationType.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update product {Name} Error: {Message}.", product.Name, ex.Message);
                _notification.Add($"Failed to update product {product.Name}.", NotificationType.Unexpected);
            }
        }
    }
}
