using Spotrader.Service.Api.DependencyInjection;
using Spotrader.Service.Api.Workers;
using Spotrader.Service.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterIoCContainers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

await app.Services.ApplyMigrationsAsync();

await app.RunAsync();
