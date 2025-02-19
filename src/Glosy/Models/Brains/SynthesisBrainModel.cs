namespace Glosy.Models.Brains
{
    public abstract class SynthesisBrainModel : BrainModel
    {
        public override string PythonScriptPath { get => Path.Combine("PythonScripts", "synthesis.py"); }
        public abstract bool AddLanguageScriptArgument { get; }
    }
}
