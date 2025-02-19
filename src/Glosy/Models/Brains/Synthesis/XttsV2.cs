using Glosy.Enums;

namespace Glosy.Models.Brains.Synthesis
{
    public class XttsV2 : SynthesisBrainModel
    {
        public override string ModelPath => "tts_models/multilingual/multi-dataset/xtts_v2";

        public override BrainName DisplayName => BrainName.Kiryu;

        public override string Description => "The best one for general use.";

        public override string IconHtmlClass => "bi bi-radioactive";

        public override bool AddLanguageScriptArgument => true;
    }
}
