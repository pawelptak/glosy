namespace Glosy.Models.Brains
{
    public abstract class ConversionBrainModel : BrainModel
    {
        public override string PythonScriptPath { get => Path.Combine("PythonScripts", "conversion.py"); }
    }
}
