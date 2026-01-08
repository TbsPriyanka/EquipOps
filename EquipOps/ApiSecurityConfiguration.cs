using EquipOps.Common.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EquipOps.Common.Configuration
{
    public static class ApiSecurityConfiguration
    {
        public static IServiceCollection AddApiSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureJwt(services, configuration);
            //ConfigureSwagger(services);

            services.AddAuthorization();
            ConfigureGlobalAuthorization(services);

            return services;
        }

        private static void ConfigureJwt(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<JwtTokenHelper>();

            services.Configure<JwtSettings>(
                configuration.GetSection("JwtSettings"));

            services.AddSingleton(
                sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);

            var jwtSettings = configuration
                .GetSection("JwtSettings")
                .Get<JwtSettings>();

            if (jwtSettings == null ||
                string.IsNullOrWhiteSpace(jwtSettings.SecretKey) ||
                jwtSettings.SecretKey.Length < 32)
            {
                throw new InvalidOperationException(
                    "JwtSettings.SecretKey must be at least 32 characters");
            }

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,

                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                });
        }

        private static void ConfigureGlobalAuthorization(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            });
        }
    }
}