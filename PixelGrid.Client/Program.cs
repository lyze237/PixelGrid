using Microsoft.Net.Http.Headers;
using PixelGrid.Client;
using PixelGrid.Client.Extensions;
using PixelGrid.Client.Options;
using PixelGrid.Client.renderer;
using PixelGrid.Client.renderer.abstracts;
using PixelGrid.Client.renderer.blender;
using PixelGrid.Client.renderer.povray;
using PixelGrid.Client.Services;
using PixelGrid.Shared.Renderer;
using Refit;

var builder = Host.CreateApplicationBuilder(args);

var renderersOptions = new RendererOptions();
builder.Configuration.GetSection("Renderers").Bind(renderersOptions);

builder.Services.Configure<RendererOptions>(builder.Configuration.GetSection("Renderers"));

builder.Services.AddSingleton<RenderFactory>();
builder.Services.AddTransient<ConnectionReceiver>();

builder.Services.AddKeyedTransient<IRenderer, BlenderRenderer>(RenderType.Blender);
builder.Services.AddKeyedTransient<IRenderer, PovrayRenderer>(RenderType.Povray);

builder.Services.AddRefitClient<IApiClient>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(renderersOptions.Url);
        c.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Bearer {renderersOptions.Token}");
    });

builder.Services.AddHostedService<ConnectionWorker>();

var host = builder.Build();
host.Run();