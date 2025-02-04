﻿using System.ComponentModel.DataAnnotations;

namespace Glosy.Models
{
    public class AudioProcessingModel
    {
        public required string ModelName { get; set; }
        public IFormFile SourceFile { get; set; }
        public required IFormFile TargetFile { get; set; }

        [StringLength(224)] // This is the limit for Polish language
        public string TextPrompt { get; set; }
        public required IFormFile OutputFile { get; set; }
    }
}
