using Microsoft.EntityFrameworkCore;
using PixelGrid.Server.Configurations;
using PixelGrid.Server.Controllers;
using PixelGrid.Server.Db;
using PixelGrid.Server.Hubs;
using PixelGrid.Server.Options;
using PixelGrid.Server.Services.Jwt;

var builder = WebApplication.CreateBuilder(args);

var jwtOptions = new JwtOptions();
builder.Configuration.Bind("jwt", jwtOptions);

builder.Services.AddGrpc();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddJwtAuthentication(jwtOptions);
builder.Services.AddSignalR();
builder.Services.AddSettings(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(options => options.AllowAnyOrigin());
app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<AuthController>();
app.MapHub<ChatHub>("/hubs/chat");

app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

app.Run();