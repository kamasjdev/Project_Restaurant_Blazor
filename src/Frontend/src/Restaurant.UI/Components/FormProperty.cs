using System.Diagnostics.CodeAnalysis;

namespace Restaurant.UI.Components
{
    public class FormProperty<T>
    {
        [NotNull] public T Value { get; set; }
        public string? Error { get; set; }
        public IEnumerable<Func<T, string>>? Rules { get; set; } = new List<Func<T, string>>();

        public bool IsValid()
        {
            foreach(var rule in Rules)
            {
                var text = rule.Invoke(Value);
                
                if (!string.IsNullOrWhiteSpace(text))
                {
                    Error = text;
                    return false;
                }
            }

            Error = "";
            return true;
        }
    }
}
