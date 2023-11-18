using PixelGrid.Client;
using PixelGrid.Client.Commands;
using PixelGrid.Client.Options;
using PixelGrid.Shared.Hubs;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ConnectionWorker>();

builder.Services.Configure<ConnectionOptions>(builder.Configuration.GetSection("Connection"));

builder.Services.AddSingleton<ITestHub.ITestHubClient, TestHubReceiver>();

var host = builder.Build();
host.Run();