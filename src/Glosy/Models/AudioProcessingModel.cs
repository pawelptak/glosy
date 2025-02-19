using Glosy.Attributes;
using Glosy.Constants;
using Glosy.Models.Brains;
using Glosy.Models.Brains.Conversion;
using Glosy.Models.Brains.Synthesis;
using System.ComponentModel.DataAnnotations;

namespace Glosy.Models
{
    public class AudioProcessingModel // TODO: Use abstraction for conversion/synthesis model instead of duplicating code
    {

        private static List<ConversionBrainModel> ConversionBrains { get; set; } = [new OpenVoiceV2(), new OpenVoiceV1(), new FreeVc()];
        private static List<SynthesisBrainModel> SynthesisBrains { get; set; } = [new XttsV2(), new XttsV1(), new Vits(), new Bark()];

        public string SelectedConversionBrainName { get; set; } = ConversionBrains.First().DisplayName.ToString();
        public string SelectedSynthesisBrainName { get; set; } = SynthesisBrains.First().DisplayName.ToString();

        public IEnumerable<BrainItem> ConversionBrainItems { get; } = ConversionBrains.Select(model => new BrainItem
        {
            Text = model.DisplayName.ToString(),
            Value = model.DisplayName.ToString(),
            Subtitle = model.Description,
            IconClass = model.IconHtmlClass
        }).ToList();

        public IEnumerable<BrainItem> SynthesisBrainItems { get; } = SynthesisBrains.Select(model => new BrainItem
        {
            Text = model.DisplayName.ToString(),
            Value = model.DisplayName.ToString(),
            Subtitle = model.Description,
            IconClass = model.IconHtmlClass
        }).ToList();


        [FileSizeLimit(AudioConstants.FileSizeLimit)]
        public IFormFile? SourceFile { get; set; } // TODO: make it required only if TextPrompt empty

        [FileSizeLimit(AudioConstants.FileSizeLimit)]
        [Required]
        public IFormFile TargetFile { get; set; }

        [StringLength(224)] // This is the model's limit for Polish language
        public string? TextPrompt { get; set; }

        public IFormFile? OutputFile { get; set; }

        public ConversionBrainModel GetSelectedConversionModel()
        {
            return ConversionBrains.Single(m => m.DisplayName.ToString() == SelectedConversionBrainName);
        }

        public SynthesisBrainModel GetSelectedSynthesisModel()
        {
            return SynthesisBrains.Single(m => m.DisplayName.ToString() == SelectedSynthesisBrainName);
        }
    }

    public class BrainItem
    {
        public required string Value { get; set; }
        public required string Text { get; set; }
        public required string Subtitle { get; set; }
        public required string IconClass { get; set; }
    }
}
