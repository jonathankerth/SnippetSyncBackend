using AspNetCoreRateLimit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SnippetSyncBackend.Data;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "snippetsync.db");
builder.Services.AddDbContext<SnippetContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add Rate Limiting
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*",
            Limit = 100,
            Period = "1h"
        }
    };
});
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

var app = builder.Build();

// Ensure the database file is created if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SnippetContext>();
    dbContext.Database.Migrate();

    // Log data to confirm seeding
    var tags = dbContext.Tags.ToList();
    var snippets = dbContext.CodeSnippets.Include(cs => cs.Tags).ToList();

    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Tags seeded: {TagsCount}", tags.Count);
    foreach (var tag in tags)
    {
        logger.LogInformation("Tag: {TagId}, {TagName}", tag.Id, tag.Name);
    }

    logger.LogInformation("Snippets seeded: {SnippetsCount}", snippets.Count);
    foreach (var snippet in snippets)
    {
        logger.LogInformation("Snippet: {SnippetId}, {SnippetTitle}", snippet.Id, snippet.Title);
        logger.LogInformation("Tags: {SnippetTags}", string.Join(", ", snippet.Tags.Select(t => t.Name)));
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
app.UseIpRateLimiting();
app.MapControllers();
app.MapGet("/health", () => "Healthy"); // Health check endpoint

var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
app.Run($"http://*:{port}");
