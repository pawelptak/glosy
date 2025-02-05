using Glosy.Interfaces;
using Glosy.Models;
using System.Diagnostics;

namespace Glosy.Services
{
    public class AudioProcessingService : IAudioProcessingService
    {
        private readonly string _pythonPath;
        private readonly string _ffmpegPath;
        private readonly string _outputDirectory = @"Multimedia";

        public AudioProcessingService(IConfiguration configuration)
        {
            _pythonPath = configuration["Config:PythonPath"];
            _ffmpegPath = configuration["Config:FFmpegPath"];
        }

        public async Task<string> ConvertVoiceAsync(AudioProcessingModel model)
        {
            var scriptPath = @"PythonScripts\conversion.py";
            model.ModelName = "voice_conversion_models/multilingual/multi-dataset/openvoice_v2";

            var sourceFilePath = Path.Combine(_outputDirectory, Path.GetRandomFileName() + Path.GetExtension(model.SourceFile.FileName));
            using (var sourceStream = new FileStream(sourceFilePath, FileMode.Create))
            {
                await model.SourceFile.CopyToAsync(sourceStream);
            }

            if (model.SourceFile.ContentType == "audio/webm")
            {
                var convertedFileName = "pies.wav";
                var convertedFilePath = Path.Combine(Path.GetDirectoryName(sourceFilePath), convertedFileName);
                ConvertWebmToWav(sourceFilePath, convertedFilePath);

                sourceFilePath = convertedFilePath;
            }

            var targetFilePath = Path.Combine(_outputDirectory, Path.GetRandomFileName() + Path.GetExtension(model.TargetFile.FileName));
            using (var targetStream = new FileStream(targetFilePath, FileMode.Create))
            {
                await model.TargetFile.CopyToAsync(targetStream);
            }

            var outputFilePath = Path.Combine("generated", "out.wav");
            var fullOutputFilePath = Path.Combine("wwwroot", outputFilePath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullOutputFilePath));

            var arguments = $"{model.ModelName} {sourceFilePath} {targetFilePath} {fullOutputFilePath}";

            var result = RunPythonScript(_pythonPath, scriptPath, arguments);

            return outputFilePath;
        }

        public async Task<string> SynthesizeVoiceAsync(AudioProcessingModel model)
        {
            var scriptPath = @"PythonScripts\synthesis.py";
            model.ModelName = "tts_models/multilingual/multi-dataset/xtts_v2";

            var targetFilePath = Path.Combine(_outputDirectory, Path.GetRandomFileName() + Path.GetExtension(model.TargetFile.FileName));
            using (var targetStream = new FileStream(targetFilePath, FileMode.Create))
            {
                await model.TargetFile.CopyToAsync(targetStream);
            }

            var outputFilePath = Path.Combine("generated", "out.wav");
            var fullOutputFilePath = Path.Combine("wwwroot", outputFilePath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullOutputFilePath));

            var language = "pl";
            var arguments = $"{model.ModelName} \"{model.TextPrompt}\" {targetFilePath} {language} {fullOutputFilePath}";

            var result = RunPythonScript(_pythonPath, scriptPath, arguments);

            return outputFilePath;
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

        private void ConvertWebmToWav(string inputPath, string outputPath)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = _ffmpegPath,
                Arguments = $"-y -i {inputPath} {outputPath}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                //UseShellExecute = false,
                //CreateNoWindow = true
            };

            using (Process ff = new Process { StartInfo = psi })
            {
                var output = new List<string>();
                var errors = new List<string>();

                ff.OutputDataReceived += (sender, e) => { if (e.Data != null) output.Add(e.Data); };
                ff.ErrorDataReceived += (sender, e) => { if (e.Data != null) errors.Add(e.Data); };

                ff.Start();
                //ff.BeginOutputReadLine();
                //ff.BeginErrorReadLine();

                ff.WaitForExit();

                foreach (var line in output)
                {
                    Console.WriteLine(line);
                }
                //if (errors.Count > 0)
                //{
                //    throw new Exception($"Ffmpeg encountered errors:\n{string.Join("\n", errors)}");
                //}
            }
        }
    }
}
