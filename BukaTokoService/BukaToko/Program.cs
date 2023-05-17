//using BukaToko.ASyncService;
using BukaToko.AsyncService;
using BukaToko.ASyncService;
using BukaToko.Data;
using BukaToko.Event;
using BukaToko.Models;
using BukaToko.SyncService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// db
var constr = builder.Configuration.GetConnectionString("MyDB");
builder.Services.AddDbContext<BukaTokoDbContext>(
    options => options.UseSqlServer(constr));

// jwt
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
// policy
var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder(
        JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
builder.Services.AddAuthorization(o => o.DefaultPolicy = policy);

builder.Services.AddControllers().AddJsonOptions(opts =>
    opts.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<IAccountRepo, AccountRepo>();
builder.Services.AddScoped<IWalletRepo, WalletRepo>();
builder.Services.AddHttpClient<IGooleDataClient, HttpGooleDataClient>();
builder.Services.AddSingleton<IEventProccessor, EventProccessor>();
builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddSingleton<IEventProccessor, EventProccessor>();
builder.Services.AddScoped<IMessageBusClient, MessageBusClient>();
//builder.Services.AddHostedService<MessageBusSubscriber>();
builder.Services.AddMvc()
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
