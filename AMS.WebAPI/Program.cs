using AMS.WebAPI.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace AMS.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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
            });

        builder.Services.AddControllers();

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}