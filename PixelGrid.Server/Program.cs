using Microsoft.EntityFrameworkCore;
using PixelGrid.Server.Configurations;
using PixelGrid.Server.Controllers;
using PixelGrid.Server.Database;
using PixelGrid.Server.Hubs;
using PixelGrid.Server.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterDependencies();

var jwtOptions = new JwtOptions();
builder.Configuration.Bind("jwt", jwtOptions);

builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddJwtAuthentication(jwtOptions);
builder.Services.AddSignalR();
builder.Services.AddSettings(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerThings(builder.Configuration);
builder.Services.AddCors();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

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

app.MapControllers();
app.MapGrpcService<ChunksController>();
app.MapHub<RenderHub>("/hubs/render");

app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

app.Run();