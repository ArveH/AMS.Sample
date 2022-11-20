using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;
using AMS.WebApp.Data;
using AMS.WebApp.Pages;

namespace AMS.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton(new Globals());
        builder.Services.AddHttpClient();
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = builder.Configuration.GetValue<string>("Auth:Authority");
                options.ClientId = builder.Configuration.GetValue<string>("Auth:ClientId");
                options.ClientSecret = builder.Configuration.GetValue<string>("Auth:ClientSecret");
                options.ResponseType = "code";
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("u4am-public-api");
                var apiName = builder.Configuration.GetValue<string>("Auth:ApiName");
                ArgumentException.ThrowIfNullOrEmpty(apiName);
                options.Scope.Add(apiName);
                options.SaveTokens = true;
                var tenantId = builder.Configuration.GetValue<string>("Auth:TenantId");
                options.Events.OnRedirectToIdentityProvider = context =>
                {
                    if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
                    {
                        context.ProtocolMessage.AcrValues = $"tenant:{tenantId}";
                    }
                    return Task.CompletedTask;
                };
            });
        builder.Services.AddRazorPages(options =>
        {
            options.Conventions.AuthorizePage("/Secure");
        });

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}