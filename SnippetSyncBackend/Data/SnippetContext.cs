using Microsoft.EntityFrameworkCore;
using SnippetSyncBackend.Models;

namespace SnippetSyncBackend.Data
{
    public class SnippetContext : DbContext
    {
        public DbSet<CodeSnippet> CodeSnippets { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public SnippetContext(DbContextOptions<SnippetContext> options) : base(options) { }
    }
}
