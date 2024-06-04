using Microsoft.EntityFrameworkCore;
using SnippetSyncBackend.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace SnippetSyncBackend.Data
{
    public class SnippetContext : DbContext
    {
        private readonly ILogger<SnippetContext> _logger;

        public SnippetContext(DbContextOptions<SnippetContext> options, ILogger<SnippetContext> logger) : base(options)
        {
            _logger = logger;
        }

        public DbSet<CodeSnippet> CodeSnippets { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Name = "C#" },
                new Tag { Id = 2, Name = "JavaScript" },
                new Tag { Id = 3, Name = "Python" }
            );

            modelBuilder.Entity<CodeSnippet>().HasData(
                new CodeSnippet
                {
                    Id = 1,
                    Title = "Hello World in C#",
                    Code = "Console.WriteLine(\"Hello, World!\");",
                    Language = "C#"
                },
                new CodeSnippet
                {
                    Id = 2,
                    Title = "Hello World in JavaScript",
                    Code = "console.log(\"Hello, World!\");",
                    Language = "JavaScript"
                },
                new CodeSnippet
                {
                    Id = 3,
                    Title = "Hello World in Python",
                    Code = "print(\"Hello, World!\")",
                    Language = "Python"
                }
            );

            modelBuilder.Entity<CodeSnippet>()
                .HasMany(cs => cs.Tags)
                .WithMany(t => t.CodeSnippets)
                .UsingEntity<Dictionary<string, object>>(
                    "CodeSnippetTag",
                    cs => cs.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                    t => t.HasOne<CodeSnippet>().WithMany().HasForeignKey("CodeSnippetId"),
                    je =>
                    {
                        je.HasData(
                            new { CodeSnippetId = 1, TagId = 1 },
                            new { CodeSnippetId = 2, TagId = 2 },
                            new { CodeSnippetId = 3, TagId = 3 }
                        );
                    });

            _logger.LogInformation("Seed data added in OnModelCreating.");
        }

        public override int SaveChanges()
        {
            _logger.LogInformation("Saving changes to the database...");
            return base.SaveChanges();
        }
    }
}
