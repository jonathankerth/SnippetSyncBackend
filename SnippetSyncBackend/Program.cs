using Microsoft.EntityFrameworkCore;
using SnippetSyncBackend.Data;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "snippetsync.db");
builder.Services.AddDbContext<SnippetContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure the database file is created if it doesn't exist
if (!File.Exists(dbPath))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<SnippetContext>();
        dbContext.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SnippetSync API V1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root (http://localhost:8080/)
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => "Healthy"); // Health check endpoint
app.Run("http://*:80");
