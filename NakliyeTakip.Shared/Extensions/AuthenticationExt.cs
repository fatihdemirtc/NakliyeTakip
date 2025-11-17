using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NakliyeTakip.Shared.Options;
using System.Security.Claims;

namespace NakliyeTakip.Shared.Extensions;

public static class AuthenticationExt
{
    public static IServiceCollection AddAuthenticationAndAuthorizationExt(this IServiceCollection services,
        IConfiguration configuration)
    {
        var identityOptions = configuration.GetSection(nameof(IdentityOption)).Get<IdentityOption>();
        //AUTHENTICATION 
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Authority = identityOptions.Address;
            options.Audience = identityOptions.Audience;
            options.RequireHttpsMetadata = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidateIssuer = true,
                RoleClaimType = ClaimTypes.Role,
                NameClaimType = ClaimTypes.NameIdentifier
            };


            // AutomaticRefreshInterval: otomatik aralıkla metadata/JWKS yenileme (ör. 24saat)
            options.AutomaticRefreshInterval = TimeSpan.FromHours(24);

            // RefreshInterval: RequestRefresh() çağrıldıktan sonra beklenen min süre (ör. 30s)
            options.RefreshInterval = TimeSpan.FromSeconds(30);
        })
        .AddJwtBearer("ClientCredentialSchema", options =>
        {
            options.Authority = identityOptions.Address;
            options.Audience = identityOptions.Audience;
            options.RequireHttpsMetadata = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidateIssuer = true
            };
        });
        //AUTHORIZATION POLICIES
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy =>
            {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypes.Email);
                policy.RequireRole(ClaimTypes.Role, "admin");
            });


            options.AddPolicy("Password", policy =>
            {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypes.Email);
            });

            options.AddPolicy("ClientCredential", policy =>
            {
                policy.AuthenticationSchemes.Add("ClientCredentialSchema");
                policy.RequireAuthenticatedUser();
            });
        });


        return services;
    }
}
