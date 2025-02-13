using System.ComponentModel.DataAnnotations;

namespace Glosy.Attributes
{
    public class FileSizeLimitAttribute : ValidationAttribute
    {
        private readonly long _maxSize;

        public FileSizeLimitAttribute(long maxSize)
        {
            _maxSize = maxSize;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file && file.Length > _maxSize)
            {
                return new ValidationResult($"File size must not exceed {_maxSize / 1024 / 1024} MB.");
            }

            return ValidationResult.Success;
        }
    }
}
