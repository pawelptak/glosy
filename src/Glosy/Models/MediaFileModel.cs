using Glosy.Enums;
using System.ComponentModel.DataAnnotations;

namespace Glosy.Models
{
    public class MediaFileModel
    {
        private string _path;
        public MediaFileModel(string path) => Path = path;

        [Required]
        public string Path
        {
            get => _path;
            set => _path = value;
        }

        public Format Format { get; set; }
    }
}
