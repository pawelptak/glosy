using Glosy.Enums;

namespace Glosy.Models.Brains.Synthesis
{
    public class Bark : SynthesisBrainModel // TODO: consider disabling it because of perfomance
    {
        public override string ModelPath => "tts_models/multilingual/multi-dataset/bark";

        public override BrainName DisplayName => BrainName.Majima;

        public override string Description => "Very slow but generates lame results.";

        public override string IconHtmlClass => "bi bi-snow2";

        public override bool AddLanguageScriptArgument => false;
    }
}
