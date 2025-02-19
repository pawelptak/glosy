using Glosy.Enums;

namespace Glosy.Models.Brains.Synthesis
{
    public class Vits : SynthesisBrainModel
    {
        public override string ModelPath => "tts_models/pl/mai_female/vits";

        public override BrainName DisplayName => BrainName.Janusz;

        public override string Description => "Not very accurate. Works best with longer audio samples.";

        public override string IconHtmlClass => "bi bi-hammer";

        public override bool AddLanguageScriptArgument => false;
    }
}
