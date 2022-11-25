

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

            //AddRateLimiter(builder, apiKeyCfg);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        /*
        private static void AddRateLimiter(WebApplicationBuilder builder, ApiKeyConfig apiKeyCfg)
        {
            var jwtPolicyName = "tokenpolicy";

            builder.Services.AddRateLimiter(limiterOptions =>
            {
                limiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                limiterOptions.AddPolicy(policyName: jwtPolicyName, partitioner: httpContext =>
                {
                    //var accessToken = httpContext.Features.Get<IAuthenticateResultFeature>()?
                    //                      .AuthenticateResult?.Properties?.GetTokenValue("access_token")?.ToString()
                    //                  ?? string.Empty;

                    var accessToken = httpContext.Request.Headers["ApiKey"];

                    if (!StringValues.IsNullOrEmpty(accessToken))
                    {
                        int permitLimit;

                        if (apiKeyCfg.Keys.Count(k => k.PrincipalName == "FreeKey") == 1)
                            permitLimit = 3;
                        else
                            permitLimit = 1000;

                        return RateLimitPartition.GetFixedWindowLimiter(accessToken, _ =>
                           new FixedWindowRateLimiterOptions
                           {
                               QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                               QueueLimit = permitLimit,
                               Window = TimeSpan.FromMinutes(1),
                               PermitLimit = permitLimit,
                               AutoReplenishment = true
                           });
                    }
                    else
                    {
                        return RateLimitPartition.GetFixedWindowLimiter(accessToken, _ =>
                           new FixedWindowRateLimiterOptions
                           {
                               QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                               QueueLimit = 0,
                               Window = TimeSpan.FromMinutes(1),
                               PermitLimit = 0,
                               AutoReplenishment = true
                           });
                    }
                });
            });
        }
        */

        private static ApiKeyConfig AddApiKeyAuth(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "ApiKey";
            }).AddScheme<ValidateApiKeyOptions, ApiKeyAuthenticationHandler>
            ("ApiKey", op =>
            {
            });

            ApiKeyConfig apiKeyCfg = new ApiKeyConfig();
            builder.Configuration.GetSection("ApiKeyConfig").Bind(apiKeyCfg);
            builder.Services.AddSingleton<ApiKeyConfig>(apiKeyCfg);
            builder.Services.AddScoped<ApiKeyAuthenticationHandler>();

            builder.Services.AddScoped<IRoleGetter, RoleGetter>();

            return apiKeyCfg;
        }
    }
}