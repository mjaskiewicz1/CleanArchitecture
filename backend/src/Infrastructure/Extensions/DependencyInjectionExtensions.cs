using System.Text;

using Application.Abstractions.Authentication;
using Application.Abstractions.Data;

using Domain.Entities.Enums;
using Domain.Repositories;

using Infrastructure.Authentication;
using Infrastructure.Authorization;
using Infrastructure.Constants;
using Infrastructure.Database;
using Infrastructure.Repositories;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Extensions;

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration, bool development)
        {
            var sqlConnectionString =
                configuration.GetConnectionString("SqlConnection") ??
                throw new InvalidOperationException("Connection string 'SqlConnection' not found.");

            var blobStorageConnectionString =
                configuration.GetConnectionString("BlobStorageConnection") ??
                throw new InvalidOperationException("Connection string 'BlobStorageConnection' not found.");

            services.AddDatabase(sqlConnectionString, development);
            services.AddAzClients(blobStorageConnectionString);
            services.AddInfraHealthChecks(sqlConnectionString);
            services.AddAuthenticationInternal(configuration);
            services.AddAuthorizationInternal();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        private void AddDatabase(string sqlConnectionString, bool development)
        {
            services.AddDbContext<ApplicationDbContext>(dbContextBuilder =>
            {
                dbContextBuilder.UseSqlServer(sqlConnectionString,
                    sqlServerDbContextBuilder =>
                        sqlServerDbContextBuilder.MigrationsHistoryTable(HistoryRepository.DefaultTableName,
                            schema: Schemas.Application));


                // ReSharper disable once InvertIf
                if (development)
                {
                    dbContextBuilder.EnableSensitiveDataLogging();
                    dbContextBuilder.EnableDetailedErrors();
                }
            });
        }

        private void AddAzClients(string blobStorageConnectionString) =>
            services.AddAzureClients(azureClientFactoryBuilder =>
                azureClientFactoryBuilder.AddBlobServiceClient(blobStorageConnectionString));

        private void AddInfraHealthChecks(string sqlConnectionString)
        {
            var healthChecksBuilder = services.AddHealthChecks();
            healthChecksBuilder.AddCheck(name: "Self", check: static () => HealthCheckResult.Healthy());
            healthChecksBuilder.AddSqlServer(sqlConnectionString, name: "Database", timeout: TimeSpan.FromSeconds(5));
            healthChecksBuilder.AddDbContextCheck<ApplicationDbContext>();
        }

        private void AddAuthorizationInternal()
        {
            services.AddAuthorization(options =>
            {
                foreach (PermissionName permission in Enum.GetValues<PermissionName>())
                {
                    options.AddPolicy(permission.ToString(),
                        policy => policy.RequireClaim("permissions", permission.ToString()));
                }
            });
        }

        private void AddAuthenticationInternal(IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = true;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });


            services.AddHttpContextAccessor();
            services.AddScoped<IUserContext, UserContext>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<ITokenProvider, TokenProvider>();
        }
    }
}