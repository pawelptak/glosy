using Glosy.Interfaces;
using Glosy.Models;
using System.Diagnostics;

namespace Glosy.Services
{
    public class AudioProcessingService : IAudioProcessingService
    {
        public async Task<FileStream> ConvertVoiceAsync(AudioConversionModel model)
        {
            var scriptPath = @"P:\\Projects\\glosy\\src\\Glosy\\PythonScripts\\conversion.py";
            model.ModelName = "voice_conversion_models/multilingual/multi-dataset/openvoice_v2";

            var tempDirectory = @"P:\\Projects\\glosy\\src\\Glosy\\Multimedia\\Output\\";
            var sourceFilePath = Path.Combine(tempDirectory, Path.GetRandomFileName() + Path.GetExtension(model.SourceFile.FileName));
            using (var sourceStream = new FileStream(sourceFilePath, FileMode.Create))
            {
                await model.SourceFile.CopyToAsync(sourceStream);
            }

            var targetFilePath = Path.Combine(tempDirectory, Path.GetRandomFileName() + Path.GetExtension(model.TargetFile.FileName));
            using (var targetStream = new FileStream(targetFilePath, FileMode.Create))
            {
                await model.TargetFile.CopyToAsync(targetStream);
            }

            var ouputFilePath = Path.Combine(tempDirectory, "out.wav");
            var arguments = $"{model.ModelName} {sourceFilePath} {targetFilePath} {ouputFilePath}";

            RunPythonScript(scriptPath, arguments);

            return new FileStream(ouputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private static void RunPythonScript(string scriptPath, string arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "python", // Make sure "python" is in PATH, or use full path e.g. "C:\\Python\\python.exe"
                Arguments = $"{scriptPath} {arguments}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
                process.ErrorDataReceived += (sender, e) => Console.WriteLine($"ERROR: {e.Data}");

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
        }
    }
}
