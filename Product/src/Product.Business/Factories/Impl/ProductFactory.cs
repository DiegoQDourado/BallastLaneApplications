using Product.Business.Entities;
using Product.Business.Models;

namespace Product.Business.Factories.Impl
{
    internal class ProductFactory : IProductFactory
    {
        public ProductEntity From(ProductModel product) =>
            new()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
            };

        public ProductModel From(ProductEntity product) =>
           new()
           {
               Id = product.Id,
               Name = product.Name,
               Description = product.Description,
               Price = product.Price,
           };

        public IEnumerable<ProductModel> From(IEnumerable<ProductEntity> product) =>
            product.Select(pe => From(pe));

    }

}
