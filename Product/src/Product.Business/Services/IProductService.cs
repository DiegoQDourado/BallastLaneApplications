using Product.Business.Models;

namespace Product.Business.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductModel>> GetAllAsync();
        Task<ProductModel?> GetByIdAsync(Guid id);
        Task CreateAsync(ProductModel product);
        Task UpdateAsync(ProductModel product);
        Task DeleteAsync(Guid id);
    }
}
