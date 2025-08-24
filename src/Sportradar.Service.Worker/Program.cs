using Sportradar.Service.Worker.DependencyInjection;
using Sportradar.Service.Worker.Workers;
using Spotrader.Service.Infrastructure.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<BettingWorker>();

builder.Services.RegisterIoCContainers();

var app = builder.Build();

await app.Services.ApplyMigrationsAsync();

await app.RunAsync();
