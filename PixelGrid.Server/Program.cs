using Microsoft.EntityFrameworkCore;
using PixelGrid.Server.Configurations;
using PixelGrid.Server.Controllers;
using PixelGrid.Server.Domain;
using PixelGrid.Server.Hubs;
using PixelGrid.Server.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterDependencies();

var jwtOptions = new JwtOptions();
builder.Configuration.Bind("jwt", jwtOptions);
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddJwtAuthentication(jwtOptions);
builder.Services.AddSignalR();
builder.Services.AddSettings(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithGrpc(builder.Configuration);
builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.EnableTryItOutByDefault();
    });
}

app.UseHttpsRedirection();
app.UseCors(options => options.AllowAnyOrigin());
app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<AuthController>();
app.MapGrpcService<RenderClientController>();
app.MapGrpcService<RenderJobController>();
app.MapGrpcService<FilesController>();

app.MapHub<RenderHub>("/hubs/render");

app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

app.Run();