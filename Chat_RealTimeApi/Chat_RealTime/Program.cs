using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Chat_RealTime.Services.user;
using Chat_RealTime.Models;
using Chat_RealTime.Controllers.chat;
using Chat_RealTime.Services.chat;
using Chat_RealTime.Hubs;
using Chat_RealTime.Services.Redis;
using StackExchange.Redis;
using Chat_RealTime.Connection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<MongoDBContext>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtServicer, JwtService>();
builder.Services.AddScoped<IChatService, ChatService>();
//builder.Services.AddScoped<IChatCacheService, ChatCacheService>();
builder.Services.AddScoped<IChatRedisService, ChatRedisService>();

//redis
//builder.Services.AddSignalR().AddStackExchangeRedis("redis:6379");
builder.Services.AddSignalR();
 

builder.Services.AddSingleton<IMongoClient>(s =>
{
    var config = s.GetRequiredService<IConfiguration>();
    var connectionString = config["MongoDb:ConnectionString"];
    return new MongoClient(connectionString);
});
builder.Services.AddAuthentication();
builder.Services.AddLocalization();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var spCultures = new[] { "en-US", "vi-VN" };
    options.SetDefaultCulture(spCultures[1])
    .AddSupportedCultures(spCultures)
    .AddSupportedUICultures(spCultures);

});
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    var connectinonString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    return ConnectionMultiplexer.Connect(connectinonString);
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            //ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                // N?u request ??n SignalR hub và có token trong query string
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };

    });

//Fe
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFE",
        policy =>
        {
            policy.WithOrigins("http://192.168.1.8:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
    
});
builder.WebHost.ConfigureKestrel(s =>
{
    s.ListenAnyIP(5231);
});


builder.Services.AddSingleton<IUserConnection, UserConnection>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseRequestLocalization();
app.UseCors("AllowFE");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
