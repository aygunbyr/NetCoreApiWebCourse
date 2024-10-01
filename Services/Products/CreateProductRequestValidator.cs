using FluentValidation;

namespace App.Services.Products
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty()
                                .WithMessage("Product name is required.")
                                .Length(3, 10)
                                .WithMessage("Product name must be between 3 and 10 characters.");

            RuleFor(x => x.Price).GreaterThan(0)
                                 .WithMessage("Product price must be greater than 0.");

            RuleFor(x => x.Stock).InclusiveBetween(1, 100)
                                 .WithMessage("Product stock quantity must be between 1 and 100.");
        }
    }
}
