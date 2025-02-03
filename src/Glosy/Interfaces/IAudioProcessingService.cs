using Glosy.Models;

namespace Glosy.Interfaces
{
    public interface IAudioProcessingService
    {
        Task<FileStream> ConvertVoiceAsync(AudioConversionModel model);
    }
}
