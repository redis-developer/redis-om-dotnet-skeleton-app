using Redis.OM;
using Redis.OM.Skeleton;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//pull connection string out of configuration
var redisConnectionString = builder.Configuration["REDIS_CONNECTION_STRING"] ?? "redis://localhost:6379";
builder.Services.AddSingleton(new RedisConnectionProvider(redisConnectionString));

// Adds startup service to create the Index
builder.Services.AddHostedService<StartupService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
