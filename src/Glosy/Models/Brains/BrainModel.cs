using Glosy.Enums;

namespace Glosy.Models.Brains
{
    public abstract class BrainModel
    {
        public abstract string ModelPath { get; }
        public abstract BrainName DisplayName { get; } // TODO: translations
        public abstract string Description { get; } // TODO: translations
        public abstract string IconHtmlClass { get; }
        public abstract string PythonScriptPath { get; }
    }
}
