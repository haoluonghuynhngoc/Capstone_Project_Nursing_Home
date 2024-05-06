using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Nursing_Home.Infrastructure;
using Nursing_Home.WebApi.Middlewares;
using Swashbuckle.AspNetCore.Filters;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Nursing_Home.WebApi;

public static class DependencyInjection
{

    public static void AddWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthenticationServices(configuration);
        services.AddSwaggerServices();
        services.ConfigureHanderException();
    }

    private static void ConfigureHanderException(this IServiceCollection services)
    {
        services.AddTransient<ExceptionHandler>();
    }

    private static void AddSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new()
            {
                Description = "JWT Authorization header using the Bearer scheme. ",
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });
            c.OperationFilter<SecurityRequirementsOperationFilter>(JwtBearerDefaults.AuthenticationScheme);
        });
    }
    private static void AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddCors();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                     configuration.GetSection("Authentication:Schemes:Bearer:SerectKey").Value!)),
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = false,
                ValidateAudience = false,
                NameClaimType = ClaimTypes.NameIdentifier
            };
            options.RequireHttpsMetadata = false;
            options.HandleEvents();
        });
    }
    private static void HandleEvents(this JwtBearerOptions options)
    {
        options.Events = new JwtBearerEvents()
        {
            OnForbidden = async context =>
            {
                var httpContext = context.HttpContext;
                var statusCode = StatusCodes.Status403Forbidden;
                var factory = httpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                var problemDetails = factory.CreateProblemDetails(httpContext: httpContext, statusCode: statusCode);

                context.Response.ContentType = MediaTypeNames.Application.Json;
                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
            },

            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.Headers.Append(HeaderNames.WWWAuthenticate, $@"{context.Options.Challenge} error = ""{context.Error}"", error_description= ""{context.ErrorDescription}"" ");

                var httpContext = context.HttpContext;
                var statusCode = StatusCodes.Status401Unauthorized;
                var factory = httpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                var problemDetails = factory.CreateProblemDetails(httpContext: httpContext, statusCode: statusCode);

                var routeData = httpContext.GetRouteData();
                var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());
                var result = new ObjectResult(problemDetails)
                {
                    StatusCode = statusCode
                };
                await result.ExecuteResultAsync(actionContext);

            },
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenDescriptor))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            }
        };
    }
    public static async Task UseWebApplication(this WebApplication app)
    {
        await app.UseInitialiseDatabaseAsync();

        // configure exception handling middleware
        app.UseMiddleware<ExceptionHandler>();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.UseCors(x => x
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(origin => true) // allow any origin
                        .AllowCredentials()); // allow credentials

        app.MapControllers();

    }
}
