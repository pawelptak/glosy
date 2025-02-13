using Glosy.Attributes;
using Glosy.Constants;
using System.ComponentModel.DataAnnotations;

namespace Glosy.Models
{
    public class AudioProcessingModel
    {
        public string? ModelName { get; set; }

        [FileSizeLimit(AudioConstants.FileSizeLimit)]
        public IFormFile? SourceFile { get; set; }

        [FileSizeLimit(AudioConstants.FileSizeLimit)]
        public required IFormFile TargetFile { get; set; }

        [StringLength(224)] // This is the model's limit for Polish language
        public string? TextPrompt { get; set; }

        public IFormFile? OutputFile { get; set; }
    }
}
