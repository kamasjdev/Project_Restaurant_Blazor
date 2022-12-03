using FluentValidation;

namespace Restaurant.UI.DTO
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public string? ProductKind { get; set; }
    }

    public class ProductValidator : AbstractValidator<ProductDto>
    {
        public ProductValidator() 
        {
            RuleFor(p => p.ProductName).NotEmpty().MinimumLength(3);
            RuleFor(p => p.Price).GreaterThanOrEqualTo(0);
            RuleFor(p => p.ProductKind).NotEmpty();
        }
    }
}
