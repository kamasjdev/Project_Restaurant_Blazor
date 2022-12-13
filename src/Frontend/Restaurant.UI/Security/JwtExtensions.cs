﻿using Restaurant.UI.DTO;
using System.Security.Claims;
using System.Text.Json;

namespace Restaurant.UI.Security
{
    public static class JwtExtensions
    {
        public static IEnumerable<Claim> ParseClaimsFromJwt(string? jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt))
            {
                return new List<Claim>();
            }

            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes) ?? new Dictionary<string, object>();
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
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
