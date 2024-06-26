using PixelGrid.Client;
using PixelGrid.Client.Options;
using PixelGrid.Client.renderer;
using PixelGrid.Client.renderer.abstracts;
using PixelGrid.Client.renderer.blender;
using PixelGrid.Client.renderer.povray;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RendererOptions>(builder.Configuration.GetSection("Renderers"));

builder.Services.AddSingleton<RenderFactory>();

builder.Services.AddKeyedTransient<IRenderer, BlenderRenderer>(RenderType.Blender);
builder.Services.AddKeyedTransient<IRenderer, PovrayRenderer>(RenderType.Povray);

builder.Services.AddHostedService<ConnectionWorker>();

var host = builder.Build();
host.Run();