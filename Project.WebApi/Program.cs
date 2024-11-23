using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Project.Common.Services.Models;
using Project.WebApi.Services;

namespace Project.WebApi;

public static class Program
{
    public static bool IsDebug;

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors();

        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("PartnersSettings"));
        var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
        var sqlDbSettings = builder.Configuration.GetSection("SqlDbSettings");

        var secretKeys = new SecretKeys();
        appSettings.IsDebug = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production";
        IsDebug = appSettings.IsDebug;
        Console.WriteLine($"IsDebug : {IsDebug}");

        secretKeys.SqlConnectionString = sqlDbSettings.GetValue<string>("ConnectionString");
        secretKeys.HashApiKey = appSettings!.Secret;

        var frontDomainList = appSettings.AllowOrigin.Split(",", StringSplitOptions.RemoveEmptyEntries);

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireClaim("Role", "1"));
            options.AddPolicy("User", policy => policy.RequireClaim("Role", "2"));
        });

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidIssuer = appSettings.ApiDomain,
                    ValidAudiences = frontDomainList,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeys.HashApiKey))
                };
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        DependenciesRegistrar.Register(appSettings, builder.Services, secretKeys);

        builder.Services.AddControllers();


        var app = builder.Build();

        if (app.Environment.IsProduction())
        {
            //Cors url only
            Console.WriteLine("AppSettings");
            Console.WriteLine(frontDomainList[0]);
            app.UseCors(x => x
                .WithOrigins(frontDomainList)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );
        }


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            Console.WriteLine("AppSettings");
            Console.WriteLine(frontDomainList[0]);
            app.UseCors(x => x
                .WithOrigins(frontDomainList)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );

            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.UseWebSockets();

        app.Map("/ws", async context =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var ws = await context.WebSockets.AcceptWebSocketAsync();
                var chatId = context.Request.Query["chatId"];

                var webSocketService = context.RequestServices.GetService<IWebSocketService>();

                if (webSocketService == null) throw new InvalidOperationException("WebSocketService not registered in DI container");

                await webSocketService.HandleWebSocketAsync(ws, int.Parse(chatId), context.RequestAborted);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        });

        await app.RunAsync();
    }
}