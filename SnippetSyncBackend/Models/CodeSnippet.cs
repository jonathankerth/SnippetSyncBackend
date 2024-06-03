namespace SnippetSyncBackend.Models
{
    public class CodeSnippet
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
}