namespace Glosy.Models
{
    public class AudioConversionModel
    {
        public required string ModelName { get; set; }          
        public required IFormFile SourceFile { get; set; }
        public required IFormFile TargetFile { get; set; }
        public required IFormFile OutputFile { get; set; }          
    }
}
