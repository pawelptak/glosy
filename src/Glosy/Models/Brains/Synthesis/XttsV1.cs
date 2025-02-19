using Glosy.Enums;

namespace Glosy.Models.Brains.Synthesis
{
    public class XttsV1 : SynthesisBrainModel
    {
        public override string ModelPath => "tts_models/multilingual/multi-dataset/xtts_v1.1";

        public override BrainName DisplayName => BrainName.Nishiki;

        public override string Description => "Kiryu's worse brother but can also achieve nice results.";

        public override string IconHtmlClass => "bi bi-virus";

        public override bool AddLanguageScriptArgument => true;
    }
}
