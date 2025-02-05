using Glosy.Interfaces;
using Glosy.Models;
using System.Diagnostics;

namespace Glosy.Services
{
    public class AudioProcessingService : IAudioProcessingService
    {
        private readonly string _pythonPath;
        private readonly string _ffmpegPath;
        private readonly string _tempFilesDirectory = @"Multimedia";

        public AudioProcessingService(IConfiguration configuration)
        {
            _pythonPath = configuration["Config:PythonPath"];
            _ffmpegPath = configuration["Config:FFmpegPath"];
        }

        public async Task<string> SynthesizeVoiceAsync(AudioProcessingModel model)
        {
            var scriptPath = @"PythonScripts\synthesis.py";
            model.ModelName = "tts_models/multilingual/multi-dataset/xtts_v2"; // TODO: assigning it to the model may not be necessary. I leave it for now.

            var outputFilePath = Path.Combine("generated", "out.wav"); // to show output file preview in the UI, the file path mustn't have the 'wwwroot' folder
            var fullOutputFilePath = Path.Combine("wwwroot", outputFilePath);

            var targetFilePath = Path.Combine(_tempFilesDirectory, model.TargetFile.FileName);

            var language = "pl";
            var arguments = $"{model.ModelName} \"{model.TextPrompt}\" {targetFilePath} {language} {fullOutputFilePath}";
            await ProcessVoice(model, scriptPath, arguments, fullOutputFilePath);

            return outputFilePath;
        }

        public async Task<string> ConvertVoiceAsync(AudioProcessingModel model)
        {
            var scriptPath = @"PythonScripts\conversion.py";
            model.ModelName = "voice_conversion_models/multilingual/multi-dataset/openvoice_v2"; // TODO: assigning it to the model may not be necessary. I leave it for now.
            var outputFileName = "out.wav";


            var sourceFilePath = Path.Combine(_tempFilesDirectory, model.SourceFile.FileName);
            await SaveStreamToDrive(sourceFilePath, model.SourceFile);

            if (string.Equals(model.SourceFile.ContentType, "audio/webm")) // Audio recorded using microphone has to be converted to mp4, otherwise it doesn't work idk why
            {
                var convertedFilePath = Path.Combine(Path.GetDirectoryName(sourceFilePath), outputFileName);
                await ConvertToWavAsync(sourceFilePath, convertedFilePath);
                sourceFilePath = convertedFilePath;
            }

            var targetFilePath = Path.Combine(_tempFilesDirectory, model.TargetFile.FileName);

            var outputFilePath = Path.Combine("generated", "out.wav"); // to show output file preview in the UI, the file path mustn't have the 'wwwroot' folder
            var fullOutputFilePath = Path.Combine("wwwroot", outputFilePath);

            var arguments = $"{model.ModelName} {sourceFilePath} {targetFilePath} {fullOutputFilePath}";
            await ProcessVoice(model, scriptPath, arguments, fullOutputFilePath);

            return outputFilePath;
        }

        private async Task<string> ProcessVoice(AudioProcessingModel model, string scriptPath, string scriptArguments, string outputFilePath)
        {
            var targetFilePath = Path.Combine(_tempFilesDirectory, model.TargetFile.FileName);
            await SaveStreamToDrive(targetFilePath, model.TargetFile);
            Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath)); // create output directory if doesn't exist

            await RunPythonScriptAsync(_pythonPath, scriptPath, scriptArguments);

            return outputFilePath;
        }

        private async Task SaveStreamToDrive(string outputPath, IFormFile file)
        {
            using var fileStream = new FileStream(outputPath, FileMode.Create);
            await file.CopyToAsync(fileStream);
        }

        private async static Task RunProcessAsync(string filePath, string arguments)
        {
            ProcessStartInfo psi = new()
            {
                FileName = filePath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new()
            { StartInfo = psi })
            {
                var output = new List<string>();
                var errors = new List<string>();

                process.OutputDataReceived += (sender, e) => { if (e.Data != null) output.Add(e.Data); };
                process.ErrorDataReceived += (sender, e) => { if (e.Data != null) errors.Add(e.Data); };

                try
                {
                    process.Start();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    throw new OperationCanceledException($"{process.ProcessName} process exited with code {process.ExitCode}");
                }
            }
        }

        private async static Task RunPythonScriptAsync(string pythonPath, string scriptPath, string scriptArguments)
        {
            await RunProcessAsync(pythonPath, $"{scriptPath} {scriptArguments}");
        }

        private async Task ConvertToWavAsync(string inputPath, string outputPath)
        {
            var arguments = $"-y -i {inputPath} {outputPath}";

            await RunProcessAsync(_ffmpegPath, arguments);
        }
    }
}
