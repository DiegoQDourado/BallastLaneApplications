using Product.Business.Entities;
using Product.Business.Models;

namespace Product.Business.Factories
{
    public interface IProductFactory
    {
        ProductEntity From(ProductModel product);
        ProductModel From(ProductEntity product);
        IEnumerable<ProductModel> From(IEnumerable<ProductEntity> product);
    }
}
