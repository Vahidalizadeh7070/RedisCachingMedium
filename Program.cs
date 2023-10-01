using System.Reflection;
using Mapster;
using MapsterMapper;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using PostService.API.Models;
using PostService.API.Services;
using PostService.API.RedisCaching;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Register AppDbContext
var SQLConnectionString = builder.Configuration.GetConnectionString("DbConnection");
builder.Services.AddDbContextPool<AppDbContext>(op=>op.UseSqlServer(SQLConnectionString));

// Register PostServices
builder.Services.AddScoped<IPostServices, PostServices>();

// Register Mapster
var config = TypeAdapterConfig.GlobalSettings;
config.Scan(Assembly.GetExecutingAssembly());

builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();

// Register Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(opt=>ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisCacheURL")));

// Register Cache service
builder.Services.AddScoped<ICacheService, CacheService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
