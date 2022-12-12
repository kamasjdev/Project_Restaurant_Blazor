using Restaurant.UI.DTO;
using System.Security.Claims;
using System.Text.Json;

namespace Restaurant.UI.Extensions
{
    public static class JwtExtensions
    {
        public static UserDto? ParseUserFromJwt(string? jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt))
            {
                return null;
            }

            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes) ?? new Dictionary<string, object>();

            var user = new UserDto();
            foreach(var kvp in keyValuePairs)
            {
                if (kvp.Key == "sub")
                {
                    user.Id = Guid.Parse(kvp.Value.ToString());
                    continue;
                }

                if (kvp.Key == ClaimTypes.Email)
                {
                    user.Email = kvp.Value.ToString();
                    continue;
                }

                if (kvp.Key == ClaimTypes.Role)
                {
                    user.Role = kvp.Value.ToString();
                    continue;
                }
            }
            return user;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
