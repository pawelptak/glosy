using Glosy.Constants;
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
        private readonly string _synthesisScriptPath = @"PythonScripts\synthesis.py";
        private readonly string _conversionScriptPath = @"PythonScripts\conversion.py";

        public AudioProcessingService(IConfiguration configuration)
        {
            _pythonPath = configuration["Config:PythonPath"];
            _ffmpegPath = configuration["Config:FFmpegPath"];
        }

        public AudioProcessingService(IConfiguration configuration, string synthesisScriptPath, string conversionScriptPath)
        {
            _pythonPath = configuration["Config:PythonPath"];
            _ffmpegPath = configuration["Config:FFmpegPath"];
            _synthesisScriptPath = synthesisScriptPath;
            _conversionScriptPath = conversionScriptPath;
        }

        public async Task<ProcessingResult> SynthesizeVoiceAsync(AudioProcessingModel model)
        {
            var scriptPath = _synthesisScriptPath;
            model.ModelName = "tts_models/multilingual/multi-dataset/xtts_v2"; // TODO: assigning it to the model may not be necessary. I leave it for now.

            var outputFilePath = Path.Combine("generated", "out.wav"); // to show output file preview in the UI, the file path mustn't have the 'wwwroot' folder
            var fullOutputFilePath = Path.Combine("wwwroot", outputFilePath);

            var targetFilePath = Path.Combine(_tempFilesDirectory, model.TargetFile.FileName);

            var language = "pl";
            var arguments = $"{model.ModelName} \"{model.TextPrompt}\" {targetFilePath} {language} {fullOutputFilePath}";

            var result = new ProcessingResult();
            try
            {
                var output = await ProcessVoice(model, scriptPath, arguments, fullOutputFilePath);
                result.IsSuccessful = true;
                result.OutputFilePath = outputFilePath;
            }
            catch (Exception ex) {
                result.ErrorMessage = ex.Message;
                Debug.WriteLine(ex, "An error occurred while processing the file.");

                throw;
            }

            return result;
        }

        public async Task<ProcessingResult> ConvertVoiceAsync(AudioProcessingModel model)
        {
            var scriptPath = _conversionScriptPath;
            model.ModelName = "voice_conversion_models/multilingual/multi-dataset/openvoice_v2"; // TODO: assigning it to the model may not be necessary. I leave it for now.

            var result = new ProcessingResult();
            try
            {
                var sourceFilePath = Path.Combine(_tempFilesDirectory, model.SourceFile.FileName);
                await SaveStreamToDrive(sourceFilePath, model.SourceFile);

                //await ConvertIfRecordedAsync(model.SourceFile, sourceFilePath); // Left it temporarily

                var targetFilePath = Path.Combine(_tempFilesDirectory, model.TargetFile.FileName);

                var outputFilePath = Path.Combine("generated", "out.wav"); // to show output file preview in the UI, the file path mustn't have the 'wwwroot' folder
                var fullOutputFilePath = Path.Combine("wwwroot", outputFilePath);

                var arguments = $"{model.ModelName} {sourceFilePath} {targetFilePath} {fullOutputFilePath}";
                var output = await ProcessVoice(model, scriptPath, arguments, fullOutputFilePath);
                result.IsSuccessful = true;
                result.OutputFilePath = outputFilePath;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                Debug.WriteLine(ex, "An error occurred while processing the file.");

                throw;
            }

            return result;
        }

        // Audio recorded using microphone has to be converted to wav, otherwise it doesn't work idk why. UPDATE: this is probably only true for text to speech
        private async Task ConvertIfRecordedAsync(IFormFile audioFile, string filePath)
        {
            if (string.Equals(audioFile.ContentType, AudioConstants.RecordingMimeType))
            {
                await ConvertToWavAsync(filePath);
            }
        }

        private async Task<string> ProcessVoice(AudioProcessingModel model, string scriptPath, string scriptArguments, string outputFilePath)
        {
            var targetFilePath = Path.Combine(_tempFilesDirectory, model.TargetFile.FileName);
            await SaveStreamToDrive(targetFilePath, model.TargetFile);
            await ConvertIfRecordedAsync(model.TargetFile, targetFilePath);

            Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath)); // create output directory if doesn't exist

            return await RunPythonScriptAsync(_pythonPath, scriptPath, scriptArguments);
        }

        private async Task SaveStreamToDrive(string outputPath, IFormFile file)
        {
            using var fileStream = new FileStream(outputPath, FileMode.Create);
            await file.CopyToAsync(fileStream);
        }

        private async static Task<string> RunProcessAsync(string filePath, string arguments)
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

                return string.Join(',', output);
            }
        }

        private async static Task<string> RunPythonScriptAsync(string pythonPath, string scriptPath, string scriptArguments)
        {
            return await RunProcessAsync(pythonPath, $"{scriptPath} {scriptArguments}");
        }

        private async Task ConvertToWavAsync(string inputPath)
        {
            var convertedFilePath = Path.Combine(Path.GetDirectoryName(inputPath), "converted.wav");
            var arguments = $"-y -i {inputPath} {convertedFilePath}";

            await RunProcessAsync(_ffmpegPath, arguments);

            File.Replace(convertedFilePath, inputPath, null);
        }
    }
}
