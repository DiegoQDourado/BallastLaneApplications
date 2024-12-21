using FluentValidation.Results;
using Product.Business.Entities.Validations;

namespace Product.Business.Entities
{
    public class ProductEntity : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        protected override ValidationResult GetValidation() =>
            new ProductValidator().Validate(this);
    }
}
