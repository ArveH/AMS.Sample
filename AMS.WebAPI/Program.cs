using AMS.WebAPI.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace AMS.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var logger = LoggerFactory.Create(config =>
        {
            config.AddConsole();
        }).CreateLogger("Program");

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Reader", policy =>
                policy.Requirements.Add(new ReaderRequirement()));
        });
        builder.Services.AddSingleton<IAuthorizationHandler, ReaderHandler>();

        builder.Services.AddAuthentication()
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = builder.Configuration.GetValue<string>("Auth:Authority");
                options.Audience = builder.Configuration.GetValue<string>("Auth:ApiName");
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        logger.LogError(context.Exception, "Authentication failed");
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddControllers();

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}