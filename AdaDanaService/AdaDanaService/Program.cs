using AdaDanaService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AdaDanaService.Data;
using Microsoft.AspNetCore.Authentication;
using AdaDanaService.DataGooleService.Http;
using AdaDanaService.AsyncDataService;
using AdaDanaService.AsyncDataServices;
using AdaDanaService.EventProcessing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// service automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// In database scaffold configuration
builder.Services.AddDbContext<AdaDanaContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Database")));


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Dependency injection repository
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();

// Konfigurasi http client dengan gole 
builder.Services.AddHttpClient<IHttpGooleService, HttpGooleService>();

builder.Services.AddSingleton<IMessageClient, MessageClient>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessageBusSubscriber>();

// service http context
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Jwt configuration
var secret = builder.Configuration["AppSettings:Secret"];
var secretBytes = Encoding.ASCII.GetBytes(secret);
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = true;
    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretBytes)
    };
});

// Configuration policy
var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder(
        JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
builder.Services.AddAuthorization(o => o.DefaultPolicy = policy);

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