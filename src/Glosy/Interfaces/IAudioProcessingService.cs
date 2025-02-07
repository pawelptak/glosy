using Glosy.Models;

namespace Glosy.Interfaces
{
    public interface IAudioProcessingService
    {
        Task<ProcessingResult> ConvertVoiceAsync(AudioProcessingModel model);
        Task<ProcessingResult> SynthesizeVoiceAsync(AudioProcessingModel model);
    }
}
