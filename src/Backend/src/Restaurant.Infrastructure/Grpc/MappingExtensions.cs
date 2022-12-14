using Restaurant.Application.DTO;
using Restaurant.Shared.OrderProto;
using Restaurant.Shared.ProductSaleProto;

namespace Restaurant.Infrastructure.Grpc
{
    internal static class MappingExtensions
    {
        public static Guid AsGuid(this string guidString)
        {
            var guid = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(guidString))
            {
                var idParsed = Guid.TryParse(guidString, out guid);
                if (idParsed is false)
                {
                    throw new ValidationException($"Entered invalid Id: '{guidString}'");
                }
            }
            return guid;
        }

        public static AdditionDto AsDto(this Shared.AdditionProto.Addition addition)
        {
            var priceParsed = decimal.TryParse(addition.Price, out var price);
            var idParsed = true;
            var id = Guid.Empty;

            if (!string.IsNullOrWhiteSpace(addition.Id))
            {
                idParsed = Guid.TryParse(addition.Id, out id);
            }

            if (priceParsed is false || idParsed is false)
            {
                var messages = new List<string>();
                if (!priceParsed)
                {
                    messages.Add($"Entered invalid Price: '{addition.Price}'");
                }
                if (!idParsed)
                {
                    messages.Add($"Entered invalid Id: '{addition.Id}'");
                }
                throw new ValidationException(messages);
            }
            var dto = new AdditionDto { Id = id, AdditionName = addition.AdditionName, Price = price, AdditionKind = addition.AdditionKind };
            return dto;
        }

        public static AddOrderDto AsDto(this AddOrderRequest request)
        {
            var errors = new List<string>();
            var productSaleIds = new List<Guid>();
            foreach(var productSaleId in request.ProductSaleIds)
            {
                var parsed = Guid.TryParse(productSaleId.Id, out var id);
                if (parsed is false)
                {
                    errors.Add($"Invalid ProductSaleId: '{productSaleId.Id}'");
                    continue;
                }
                productSaleIds.Add(id);
            }

            var orderId = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(request.Id))
            {
                var parsed = Guid.TryParse(request.Id, out orderId);
                if (parsed is false)
                {
                    errors.Add($"Entered invalid Id: '{request.Id}'");
                }
            }

            if (errors.Any())
            {
                throw new ValidationException(errors);
            }

            return new AddOrderDto { Id = orderId, Email = request.Email, Note = request.Note, ProductSaleIds = productSaleIds };
        }

        public static ProductDto AsDto(this Shared.ProductProto.Product request)
        {
            var errors = new List<string>();
            var priceParsed = decimal.TryParse(request.Price, out var price);
            if (priceParsed is false)
            {
                errors.Add($"Entered invalid Price: '{request.Price}'");
            }

            var productId = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(request.Id))
            {
                var parsed = Guid.TryParse(request.Id, out productId);
                if (parsed is false)
                {
                    errors.Add($"Entered invalid Id: '{request.Id}'");
                }
            }

            if (errors.Any())
            {
                throw new ValidationException(errors);
            }

            return new ProductDto { Id = productId, ProductName = request.ProductName, Price = price, ProductKind = request.ProductKind };
        }

        public static AddProductSaleDto AsDto(this AddProductSaleRequest request)
        {
            var errors = new List<string>();

            var productSaleId = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(request.Id))
            {
                var parsed = Guid.TryParse(request.Id, out productSaleId);
                if (parsed is false)
                {
                    errors.Add($"Entered invalid Id: '{request.Id}'");
                }
            }

            Guid? additionId = null;
            if (!string.IsNullOrWhiteSpace(request.AdditionId))
            {
                var additionIdParsed = Guid.TryParse(request.AdditionId, out var parsedAdditionId);

                if (additionIdParsed is false)
                {
                    errors.Add($"Entered invalid AdditionId: '{request.AdditionId}'");
                }
                else
                {
                    additionId = parsedAdditionId;
                }
            }

            Guid? orderId = null;
            if (!string.IsNullOrWhiteSpace(request.OrderId))
            {
                var orderIdParsed = Guid.TryParse(request.OrderId, out var parsedOrderId);

                if (orderIdParsed is false)
                {
                    errors.Add($"Entered invalid OrderId: '{request.OrderId}'");
                }
                else
                {
                    orderId = parsedOrderId;
                }
            }

            var productIdParsed = Guid.TryParse(request.ProductId, out var productId);

            if (productIdParsed is false)
            {
                errors.Add($"Entered invalid ProductId: '{request.ProductId}'");
            }

            if (errors.Any())
            {
                throw new ValidationException(errors);
            }

            return new AddProductSaleDto { Id = productSaleId, AdditionId = additionId, OrderId = orderId, Email = request.Email, ProductId = productId };
        }
    }
}
