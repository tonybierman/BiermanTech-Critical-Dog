using Markdig;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class MarkdownRendererPartialViewModel
    {
        public string FileName { get; set; }
        public string WebRootPath { get; set; }
        public string MarkdownHtml { get; private set; } = string.Empty;

        public MarkdownRendererPartialViewModel(string fileName, string webRootPath)
        {
            FileName = fileName;
            WebRootPath = webRootPath;

            // Load markdown content when the view model is instantiated
            LoadMarkdownContent();
        }

        private void LoadMarkdownContent()
        {
            if (!string.IsNullOrEmpty(FileName) && !string.IsNullOrEmpty(WebRootPath))
            {
                var markdownFilePath = System.IO.Path.Combine(WebRootPath, "help", FileName);
                if (System.IO.File.Exists(markdownFilePath))
                {
                    var markdownContent = System.IO.File.ReadAllText(markdownFilePath);
                    var pipeline = new Markdig.MarkdownPipelineBuilder()
                        .UseAdvancedExtensions()
                        .Build();
                    MarkdownHtml = Markdig.Markdown.ToHtml(markdownContent, pipeline);
                }
            }
        }
    }
}
