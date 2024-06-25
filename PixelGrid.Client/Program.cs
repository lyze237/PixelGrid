using PixelGrid.Client;

var builder = Host.CreateApplicationBuilder(args);
//builder.Services.AddHostedService<PlaygroundWorker>();
builder.Services.AddHostedService<OfflineWorker>();

var host = builder.Build();
host.Run();