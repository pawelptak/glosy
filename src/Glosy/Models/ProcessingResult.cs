namespace Glosy.Models
{
    public class ProcessingResult
    {
        public bool IsSuccessful { get; set; } = false;
        public string? OutputFilePath { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
