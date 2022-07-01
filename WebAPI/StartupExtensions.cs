using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using WebAPI.Data;
using WebAPI.Models;
using static Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults;

namespace WebAPI;

public static class StartupExtensions
{
    public static IServiceCollection AddAuthenticationAndJwt(this IServiceCollection services, IConfiguration cf)
    {
        services.AddAuthentication(configureOptions =>
            {
                configureOptions.DefaultAuthenticateScheme = AuthenticationScheme;
                configureOptions.DefaultChallengeScheme = AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.ClaimsIssuer = AuthenticationScheme;
            });
        return services;
    }
    
    public static OpenIddictBuilder AddOpenIddictServer(this IServiceCollection services, 
        IWebHostEnvironment environment)
    {
        return services
            .AddOpenIddict()
            .AddCore(options =>
            {
                options
                    .UseEntityFrameworkCore()
                    .UseDbContext<ApplicationDbContext>();
            })
            .AddServer(options =>
            {
                options
                    .AcceptAnonymousClients()
                    .AllowPasswordFlow();

                options
                    .SetTokenEndpointUris(
                        "/api/auth/signup", 
                        "/api/auth/login"
                    );
                
                options
                    .AddEphemeralEncryptionKey()
                    .AddEphemeralSigningKey();
                
                options
                    .RegisterScopes(OpenIddictConstants.Scopes.OfflineAccess);
                
                var cfg = options.UseAspNetCore();
                if (environment.IsDevelopment() || environment.IsStaging())
                {
                    cfg
                        .DisableTransportSecurityRequirement();
                    options
                        .DisableAccessTokenEncryption();
                }
                
                cfg.EnableTokenEndpointPassthrough();
                
                options
                    .AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();
            }).AddValidation(options =>
            {
                options.UseAspNetCore();
                options.UseLocalServer();
            });
    }
    
    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
            options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
            options.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;
        });

        return services;
    }
}