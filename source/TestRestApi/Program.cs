

using Daenet.ApiKeyAuthenticator;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;

namespace TestRestApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = @"Please provide the key in the value field bellow.",
                    Name = "ApiKey",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiKey"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            },
                            Scheme = "ApiKey",
                            Name = "ApiKey",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            var apiKeyCfg = AddApiKeyAuth(builder);

            // See https://github.com/ddobric/SkuRateLimiter
            //AddRateLimiter(builder, apiKeyCfg);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

    
        private static ApiKeyConfig AddApiKeyAuth(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(options =>
            {
                //options.DefaultAuthenticateScheme = "ApiKey";
            }).AddScheme<ValidateApiKeyOptions, ApiKeyAuthenticationHandler>
            ("ApiKey", op =>
            {
            });

            ApiKeyConfig apiKeyCfg = new ApiKeyConfig();
            builder.Configuration.GetSection("ApiKeyConfig").Bind(apiKeyCfg);
            builder.Services.AddSingleton<ApiKeyConfig>(apiKeyCfg);
           
            builder.Services.AddScoped<IRoleGetter, RoleGetter>();
            builder.Services.AddScoped<ICustomClaimsBuilder, ClaimsBuilder>();

            return apiKeyCfg;
        }
    }
}