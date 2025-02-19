using Glosy.Enums;

namespace Glosy.Models.Brains.Conversion
{
    public class OpenVoiceV2 : ConversionBrainModel
    {
        public override string ModelPath => "voice_conversion_models/multilingual/multi-dataset/openvoice_v2";

        public override BrainName DisplayName => BrainName.Arthur;

        public override string Description => "The best for general use. Also the fastest one.";

        public override string IconHtmlClass => "bi bi-robot";
    }
}
