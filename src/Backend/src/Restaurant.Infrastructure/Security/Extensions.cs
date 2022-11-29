using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Restaurant.Application.Abstractions;
using Restaurant.Core.Entities;
using System.Text;

namespace Restaurant.Infrastructure.Security
{
    internal static class Extensions
    {
        private const string OptionsSectionName = "auth";

        public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetOptions<AuthOptions>(OptionsSectionName);

            services
                .Configure<AuthOptions>(configuration.GetRequiredSection("auth"))
                .AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.Audience = options.Audience;
                    o.IncludeErrorDetails = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = options.Issuer,
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey))
                    };
                });

            services.AddAuthorization(authorization =>
            {
                authorization.AddPolicy("is-admin", policy =>
                {
                    policy.RequireRole("admin");
                });
            });

            services.AddSingleton<IJwtManager, JwtManager>();
            services.AddSingleton<IPasswordManager, PasswordManager>();
			services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
			return services;
        }
    }
}
