using Glosy.Models;

namespace Glosy.Interfaces
{
    public interface IAudioProcessingService
    {
        Task<string> ConvertVoiceAsync(AudioProcessingModel model);
        Task<string> SynthesizeVoiceAsync(AudioProcessingModel model);
    }
}
