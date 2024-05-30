namespace SnippetSyncBackend.Models
{
    public class CodeSnippet
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string Language { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
