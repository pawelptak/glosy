using Glosy.Enums;

namespace Glosy.Models.Brains.Conversion
{
    public class FreeVc : ConversionBrainModel
    {
        public override string ModelPath => "voice_conversion_models/multilingual/vctk/freevc24";

        public override BrainName DisplayName => BrainName.Carl;

        public override string Description => "The slowest but also the worst.";

        public override string IconHtmlClass => "bi bi-airplane";
    }
}
