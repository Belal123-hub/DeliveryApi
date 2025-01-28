using System.Security.Claims;
using System.Text;
using Backend2024ExampleApp.Configuration;
using BLL.Configuration;
using BLL.Services;
using DAL.Configurations;
using DAL.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization; // Add this for JsonStringEnumConverter
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Any;
using System.Reflection;
using DeliveryWebApi.MiddleWares; // Add this namespace

namespace Backend2024ExampleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Services
            ConfigureServices(builder);

            var app = builder.Build();

            // Apply Database Migrations
            ApplyDatabaseMigrations(app);

            // Configure Middlewares
            ConfigureMiddlewares(app);

            // Run the Application
            await app.ConfigureIdentityAsync();
            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            // Add Controllers with JSON enum string conversion
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Convert enums to strings in JSON responses
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            // Configure Swagger
            ConfigureSwagger(builder);

            // Configure Data Access Layer (DAL) and Business Logic Layer (BLL)
            builder.ConfigureDal();
            builder.ConfigureBll();
            builder.Services.AddLogging();

            // Configure JWT Authentication
            ConfigureJwtAuthentication(builder);

            // Configure Authorization Policies
            ConfigureAuthorization(builder);
        }

        private static void ConfigureSwagger(WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations(); // Enable Swagger annotations
                // Configure Swagger to display enums as strings
                options.UseAllOfToExtendReferenceSchemas();
                options.SchemaFilter<EnumSchemaFilter>(); // Add custom schema filter

                // Configure JWT authentication in Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Bearer Authentication with JWT Token",
                    Type = SecuritySchemeType.Http
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });
        }

        private static void ConfigureJwtAuthentication(WebApplicationBuilder builder)
        {
            var jwtSection = builder.Configuration.GetSection("JwtBearerTokenSettings");
            builder.Services.Configure<JwtBearerTokenSettings>(jwtSection);

            var jwtConfiguration = jwtSection.Get<JwtBearerTokenSettings>();
            var key = Encoding.ASCII.GetBytes(jwtConfiguration.secretKey);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = jwtConfiguration.Audience,
                    ValidIssuer = jwtConfiguration.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    LifetimeValidator = (before, expires, token, parameters) =>
                    {
                        var utcNow = DateTime.UtcNow;
                        return before <= utcNow && utcNow < expires;
                    }
                };
            });
        }

        private static void ConfigureAuthorization(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(ApplicationRoleNames.Administrator,
                    new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireRole(ApplicationRoleNames.Administrator)
                    .RequireClaim(ClaimTypes.Role, ApplicationRoleNames.Administrator)
                    .Build());
            });
        }

        private static void ApplyDatabaseMigrations(WebApplication app)
        {
            using var serviceScope = app.Services.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            context.Database.Migrate();
        }

        private static void ConfigureMiddlewares(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<TokenValidationMiddleWare>();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }
    }

    // Custom schema filter to display enums as strings in Swagger
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                schema.Enum.Clear();
                foreach (var value in Enum.GetNames(context.Type))
                {
                    schema.Enum.Add(new OpenApiString(value));
                }
            }
        }
    }
}