using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;

namespace Restaurant.UI.Pages
{
    public static class PageExtensions
    {
        public static string GetValidationMessage(this EditContext editContext, Expression<Func<object>> field)
        {
            var validationMessage = editContext.GetValidationMessages(field).FirstOrDefault();
            return validationMessage ?? "";
        }
    }
}
