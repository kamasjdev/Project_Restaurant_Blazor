using Restaurant.Application.Exceptions;
using Restaurant.Core.Entities;
using System.Text;

namespace Restaurant.Application.Mail
{
    public sealed class EmailMessage : IEmailMessage
    {
        private EmailMessage(string subject, string body)
        {
            Subject = subject;
            Body = body;
        }

        public string Subject { get; }
        public string Body { get; }

        public sealed class EmailMessageBuilder
        {
            private StringBuilder _subject = new();
            private StringBuilder _body = new();
            private const string startTable = "<table style=\"border-collapse:collapse; text-align:center;\" >";
            private const string endTable = "</table>";
            private const string startHeaderRow = "<tr style=\"background-color:#6FA1D2; color:#ffffff;\">";
            private const string endHeaderRow = "</tr>";
            private const string startTr = "<tr style=\"color:#555555;\">";
            private const string endTr = "</tr>";
            private const string startTd = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
            private const string endTd = "</td>";

            public EmailMessage ConstructEmailFromOrder(Order order)
            {
                if (order == null)
                {
                    throw new CannotConstructEmailFromOrderException();
                }

                _subject.Append($"Zamówienie nr {order.OrderNumber.Value}");
                _body.Append("<font>Nr zamówienia: ");
                _body.Append(order.OrderNumber.Value);
                _body.Append(", data zamówienia: ");
                _body.Append(order.Created);
                _body.Append("</font>");
                AddProducts(order.Products);

                if (order.Note is not null)
                {
                    AddNotes(order.Note);
                }

                _body.Append("<br/>");
                _body.AppendLine("\n<font>Koszt : " + order.Price.Value + " zł" + "</font>");

                return new EmailMessage(_subject.ToString(), _body.ToString());
            }

            private void AddProducts(IEnumerable<ProductSale> products)
            {
                if (products is null)
                {
                    return;
                }

                if (!products.Any())
                {
                    return;
                }

                _body.Append("<br><br>");
                _body.Append(startTable);
                _body.Append(startHeaderRow);
                _body.Append(startTd + "Nazwa Dania" + endTd);
                _body.Append(startTd + "Koszt" + endTd);
                _body.Append(endHeaderRow);

                foreach (var productSale in products)
                {
                    AddProduct(productSale);
                }

                _body.Append(endTable);
                _body.Append("<br/>");
            }

            private void AddProduct(ProductSale productSale)
            {
                var product = new StringBuilder(startTr);
                product.Append(startTd);
                product.Append(productSale.Product.ProductName.Value);
                product.Append(endTd);
                product.Append(startTd);
                product.Append(productSale.Product.Price.Value);
                product.Append(" zł");
                product.Append(endTd);
                product.Append(endTr);

                if (productSale.Addition != null)
                {
                    product.Append(startTr);
                    product.Append(startTd);
                    product.Append(productSale.Addition.AdditionName);
                    product.Append(" zł");
                    product.Append(endTd);
                    product.Append(startTd);
                    product.Append(productSale.Addition.Price);
                    product.Append(" zł");
                    product.Append(endTd);
                    product.Append(endTr);
                }

                _body.Append(product);
            }

            private void AddNotes(string notes)
            {
                var note = new StringBuilder("<h5>");
                note.Append("Uwagi:");
                note.Append("</h5>");
                note.Append("<h5>");
                note.Append(notes);
                note.Append("</h5>");
                _body.Append(note);
            }
        }
    }
}
