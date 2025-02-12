using Glosy.Constants;
using Glosy.Interfaces;
using Glosy.Models;
using System.Diagnostics;

namespace Glosy.Services
{
    public class AudioProcessingService : IAudioProcessingService
    {
        private readonly ILogger<AudioProcessingService> _logger;
        private readonly string _pythonPath;
        private readonly string _ffmpegPath;
        private readonly string _tempFilesDirectory = @"Multimedia";
        private readonly string _synthesisScriptPath = @"PythonScripts\synthesis.py";
        private readonly string _conversionScriptPath = @"PythonScripts\conversion.py";

        public AudioProcessingService(ILogger<AudioProcessingService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _pythonPath = configuration["Config:PythonPath"];
            _ffmpegPath = configuration["Config:FFmpegPath"];

            Directory.CreateDirectory(_tempFilesDirectory);
        }

        public AudioProcessingService(ILogger<AudioProcessingService> logger, IConfiguration configuration, string tempFilesDirectory, string synthesisScriptPath, string conversionScriptPath)
        {
            _logger = logger;
            _pythonPath = configuration["Config:PythonPath"];
            _ffmpegPath = configuration["Config:FFmpegPath"];
            _synthesisScriptPath = synthesisScriptPath;
            _conversionScriptPath = conversionScriptPath;
            _tempFilesDirectory = tempFilesDirectory;
        }

        public async Task<ProcessingResult> SynthesizeVoiceAsync(AudioProcessingModel model)
        {
            _logger.LogInformation("Executing {FunctionName} at {DateTime}. Target: {TargetFile}", nameof(SynthesizeVoiceAsync), DateTime.UtcNow, model.TargetFile.FileName);

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
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                Debug.WriteLine(ex, "An error occurred while processing the file.");

                throw;
            }
            finally
            {
                ClearDirectory(_tempFilesDirectory);
            }

            return result;
        }

        public async Task<ProcessingResult> ConvertVoiceAsync(AudioProcessingModel model)
        {
            _logger.LogInformation("Executing {FunctionName} at {DateTime}. Source: {SourceFile}, Target: {TargetFile}", nameof(ConvertVoiceAsync), DateTime.UtcNow, model.SourceFile.FileName, model.TargetFile.FileName);

            var scriptPath = _conversionScriptPath;
            model.ModelName = "voice_conversion_models/multilingual/multi-dataset/openvoice_v2"; // TODO: assigning it to the model may not be necessary. I leave it for now.

            var result = new ProcessingResult();
            try
            {
                var sourceFilePath = Path.Combine(_tempFilesDirectory, model.SourceFile.FileName);
                await SaveStreamToDrive(sourceFilePath, model.SourceFile);

                //await ConvertIfRecordedAsync(model.SourceFile, sourceFilePath); // Left it temporarily
                await ConvertIfMimeTypes(model.SourceFile, sourceFilePath, [AudioConstants.Mp3MimeType]);

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
            //finally
            //{
            //    ClearDirectory(_tempFilesDirectory);
            //}

            return result;
        }

        // Audio recorded using microphone has to be converted to wav, otherwise it doesn't work idk why. UPDATE: this is probably only true for text to speech
        private async Task ConvertIfMimeTypes(IFormFile audioFile, string filePath, string[] convertedMimeTypes)
        {
            if (convertedMimeTypes.Contains(audioFile.ContentType))
            {
                await ConvertToWavAsync(filePath);
            }
        }

        private async Task<string> ProcessVoice(AudioProcessingModel model, string scriptPath, string scriptArguments, string outputFilePath)
        {
            _logger.LogInformation("Executing {FunctionName} at {DateTime}", nameof(ProcessVoice), DateTime.UtcNow);

            var targetFilePath = Path.Combine(_tempFilesDirectory, model.TargetFile.FileName);
            await SaveStreamToDrive(targetFilePath, model.TargetFile);
            await ConvertIfMimeTypes(model.TargetFile, targetFilePath, [AudioConstants.RecordingMimeType, AudioConstants.Mp3MimeType]);

            Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath)); // create output directory if doesn't exist

            return await RunPythonScriptAsync(_pythonPath, scriptPath, scriptArguments);
        }

        private async Task SaveStreamToDrive(string outputPath, IFormFile file)
        {
            _logger.LogInformation("Executing {FunctionName} at {DateTime}. Saving {FileName} ({Size}) to {Path}", nameof(SaveStreamToDrive), DateTime.UtcNow, file.FileName, file.Length, outputPath);

            try
            {
                using var fileStream = new FileStream(outputPath, FileMode.Create);
                await file.CopyToAsync(fileStream);
                fileStream.Close();

                bool fileExists = File.Exists(outputPath);
                _logger.LogInformation("File {FileName} saved successfully: {Exists}", outputPath, fileExists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing file to disk: {FileName}", outputPath);
                throw;
            }
        }

        private async Task<string> RunProcessAsync(string filePath, string arguments)
        {
            _logger.LogInformation("Executing {Process} {Arguments} at {DateTime}", filePath, arguments, DateTime.UtcNow);

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
                    _logger.LogError("{Process} exception: {ExceptionMessage}", filePath, ex.Message);

                    throw new InvalidOperationException(ex.Message);
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    _logger.LogError("{Process} error: {Output}", filePath, string.Join(',', output));

                    throw new OperationCanceledException($"{process.ProcessName} process exited with code {process.ExitCode}");
                }

                return string.Join(',', output);
            }
        }

        private async Task<string> RunPythonScriptAsync(string pythonPath, string scriptPath, string scriptArguments)
        {
            return await RunProcessAsync(pythonPath, $"{scriptPath} {scriptArguments}");
        }

        private async Task ConvertToWavAsync(string inputPath)
        {
            var convertedFilePath = Path.Combine(Path.GetDirectoryName(inputPath), "converted.wav");
            var arguments = $"-y -i {inputPath} {convertedFilePath}";

            _logger.LogInformation("Executing {FunctionName} at {DateTime}", nameof(ConvertToWavAsync), DateTime.UtcNow);

            await RunProcessAsync(_ffmpegPath, arguments);

            File.Replace(convertedFilePath, inputPath, null);
        }

        private void ClearDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            DirectoryInfo di = new(directory);

            foreach (var file in di.GetFiles())
            {
                if (!file.Name.EndsWith(".gitkeep"))
                {
                    file.Delete();
                }
            }
        }
    }
}
