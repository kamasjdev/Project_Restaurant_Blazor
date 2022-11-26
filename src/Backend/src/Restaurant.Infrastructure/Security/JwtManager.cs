using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Restaurant.Application.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Restaurant.Infrastructure.Security
{
    internal sealed class JwtManager : IJwtManager
    {
        private readonly IClock _clock;
        private readonly string _issuer;
        private readonly TimeSpan _expiry;
        private readonly string _audience;
        private readonly SigningCredentials _signingCredentials;
        private readonly JwtSecurityTokenHandler _jwtSecurityToken = new JwtSecurityTokenHandler();

        public JwtManager(IOptions<AuthOptions> options, IClock clock)
        {
            _clock = clock;
            _issuer = options.Value.Issuer;
            _audience = options.Value.Audience;
            _expiry = options.Value.Expiry ?? TimeSpan.FromHours(1);
            _signingCredentials = new SigningCredentials(new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(options.Value.SigningKey)),
                    SecurityAlgorithms.HmacSha256);
        }

        public string CreateToken(Guid userId, string role)
        {
            var now = _clock.CurrentDate();
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
                new(ClaimTypes.Role, role)
            };

            var expires = now.Add(_expiry);
            var jwt = new JwtSecurityToken(_issuer, _audience, claims, now, expires, _signingCredentials);
            var token = _jwtSecurityToken.WriteToken(jwt);

            return token;
        }
    }
}
