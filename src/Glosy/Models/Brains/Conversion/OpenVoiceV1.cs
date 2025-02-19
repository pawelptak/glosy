using Glosy.Enums;

namespace Glosy.Models.Brains.Conversion
{
    public class OpenVoiceV1 : ConversionBrainModel
    {
        public override string ModelPath => "voice_conversion_models/multilingual/multi-dataset/openvoice_v1";

        public override BrainName DisplayName => BrainName.John;

        public override string Description => "Slower than Arthur but worse.";

        public override string IconHtmlClass => "bi bi-rocket";
    }
}
