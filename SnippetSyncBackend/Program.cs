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

var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "snippetsync.db");
builder.Services.AddDbContext<SnippetContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SnippetContext>();
    dbContext.Database.Migrate();

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

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SnippetSync API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => "Healthy");

var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
app.Run($"http://*:{port}");
