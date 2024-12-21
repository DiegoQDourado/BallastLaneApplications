using Product.Business.Entities;

namespace Product.Business.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductEntity>> GetAllAsync();
        Task<ProductEntity> GetByAsync(Guid id);
        Task<ProductEntity> GetByOrAsync(Guid id, string name);
        Task AddAsync(ProductEntity product);
        Task UpdateAsync(ProductEntity product);
        Task<bool> DeleteAsync(Guid id);
    }
}
