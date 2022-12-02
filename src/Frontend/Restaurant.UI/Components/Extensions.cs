using Microsoft.AspNetCore.Components.Rendering;

namespace Restaurant.UI.Components
{
    public static class Extensions
    {
        public static void AddAttributeIfNotNullOrEmpty(this RenderTreeBuilder builder, int sequence, string name, string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                builder.AddAttribute(sequence, name, value);
            }
        }
    }
}
