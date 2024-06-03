namespace SnippetSyncBackend.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<CodeSnippet> CodeSnippets { get; set; } = new List<CodeSnippet>();
    }
}