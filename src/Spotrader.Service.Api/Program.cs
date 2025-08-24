using Spotrader.Service.Api.DependencyInjection;
using Spotrader.Service.Api.Services;
using Spotrader.Service.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register seeding service
builder.Services.AddScoped<DataSeedingService>();

builder.Services.RegisterIoCContainers(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.Services.ApplyMigrationsAsync();

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();
