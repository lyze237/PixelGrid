using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PixelGrid.Database;
using PixelGrid.Server.Configurations;
using PixelGrid.Server.Controllers;
using PixelGrid.Server.Domain;
using PixelGrid.Server.Hubs;
using PixelGrid.Server.Options;

var builder = WebApplication.CreateBuilder(args);

var jwtOptions = new JwtOptions();
builder.Configuration.Bind("jwt", jwtOptions);

builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddJwtAuthentication(jwtOptions);
builder.Services.AddSignalR();
builder.Services.AddSettings(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(config =>
{
    var filePath = Path.Combine(AppContext.BaseDirectory, "PixelGrid.Server.xml");
    config.IncludeXmlComments(filePath);
    config.IncludeGrpcXmlComments(filePath, includeControllerXmlComments: true);

    config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    config.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
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
app.MapGrpcService<ClientController>();
app.MapHub<ChatHub>("/hubs/chat");
app.MapHub<ClientHub>("/hubs/client");

app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

app.Run();