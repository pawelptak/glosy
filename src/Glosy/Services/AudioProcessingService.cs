using Glosy.Interfaces;
using Glosy.Models;
using System.Diagnostics;

namespace Glosy.Services
{
    public class AudioProcessingService : IAudioProcessingService
    {
        private readonly string _pythonPath;

        public AudioProcessingService(IConfiguration configuration)
        {
                _pythonPath = configuration["Config:PythonPath"];
        }

        public async Task<FileStream> ConvertVoiceAsync(AudioConversionModel model)
        {
            var scriptPath = @"PythonScripts\conversion.py";
            model.ModelName = "voice_conversion_models/multilingual/multi-dataset/openvoice_v2";

            var tempDirectory = @"Multimedia\Output";
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

            var result = RunPythonScript(_pythonPath, scriptPath, arguments);

            return new FileStream(ouputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private static string RunPythonScript(string pythonPath, string scriptPath, string arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = pythonPath,
                Arguments = $"{scriptPath} {arguments}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = psi })
            {
                var output = new List<string>();
                var errors = new List<string>();

                process.OutputDataReceived += (sender, e) => { if (e.Data != null) output.Add(e.Data); };
                process.ErrorDataReceived += (sender, e) => { if (e.Data != null) errors.Add(e.Data); };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();

                if (errors.Count > 0)
                {
                    throw new Exception($"Python script encountered errors:\n{string.Join("\n", errors)}");
                }

                return string.Join("\n", output);
            }
        }

    }
}
