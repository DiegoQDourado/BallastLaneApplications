using FluentValidation;

namespace Product.Business.Entities.Validations
{
    public class ProductValidator : AbstractValidator<ProductEntity>
    {

        public ProductValidator()
        {
            RuleFor(a => a.Id)
                .NotEmpty()
                .WithMessage("Product Id could not be empty.");

            RuleFor(a => a.Name)
                .NotEmpty()
                .WithMessage("Product Name could not be empty.");

            RuleFor(a => a.Description)
               .NotEmpty()
               .WithMessage("Product Description could not be empty.");

            RuleFor(a => a.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greather than zero.");
        }
    }
}
