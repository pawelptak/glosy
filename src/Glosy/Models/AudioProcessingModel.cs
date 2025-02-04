namespace Glosy.Models
{
    public class AudioProcessingModel
    {
        public required string ModelName { get; set; }
        public IFormFile SourceFile { get; set; }
        public required IFormFile TargetFile { get; set; }
        public string TextPrompt { get; set; }
        public required IFormFile OutputFile { get; set; }
    }
}
