using Glosy.Models;
using Glosy.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Glosy.Tests
{
    public class UnitTests
    {
        private readonly AudioProcessingService _audioProcessingService;
        private readonly string _synthesisModel = "tts_models/multilingual/multi-dataset/xtts_v2";
        private readonly string _conversionModel = "voice_conversion_models/multilingual/multi-dataset/openvoice_v2";
        private readonly string _synthesisScriptPath;
        private readonly string _conversionScriptPath;
        private readonly string _tempFilesDirectory = "Multimedia";
        private readonly string _testFilesDirectory = "TestFiles";

        public UnitTests()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var solutionDirectory = Directory.GetParent(currentDirectory).Parent.Parent.Parent.FullName;
            var projectDirectory = Path.Combine(solutionDirectory, "Glosy");
            var ffmpegPath = Path.Combine(projectDirectory, "Utilities", "ffmpeg.exe");
            _synthesisScriptPath = Path.Combine(projectDirectory, "PythonScripts", "synthesis.py");
            _conversionScriptPath = Path.Combine(projectDirectory, "PythonScripts", "conversion.py");

            var inMemorySettings = new Dictionary<string, string>
            {
                { "Config:FFmpegPath", ffmpegPath },
                { "Config:PythonPath", "P:/Projects/glosy/venv/Scripts/python.exe" }, // Replace with your path
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            var logger = loggerFactory.CreateLogger<AudioProcessingService>();

            _audioProcessingService = new AudioProcessingService(logger, configuration, _tempFilesDirectory, _synthesisScriptPath, _conversionScriptPath);

        }


        [Fact]
        public async Task Synthesis_Output_File_Should_Have_Size_Larger_Than_Zero()
        {
            // Arrange
            var textPrompt = "Witaj przyjacielu.";
            var targetFilePath = GetTestProjectAbsoluteFilePath(Path.Combine(_testFilesDirectory, "zebrowski.wav"));
            var targetFile = CreateFormFileFromPath(targetFilePath);
            var model = new AudioProcessingModel { ModelName = _synthesisModel, TextPrompt = textPrompt, TargetFile = targetFile };

            // Act
            var result = await _audioProcessingService.SynthesizeVoiceAsync(model);
            var fileInfo = new FileInfo(Path.Join("wwwroot", result.OutputFilePath));

            // Assert
            Assert.True(fileInfo.Length > 0);
        }

        [Fact]
        public async Task Conversion_Output_File_Should_Have_Size_Larger_Than_Zero()
        {
            // Arrange
            var sourceFilePath = GetTestProjectAbsoluteFilePath(Path.Combine(_testFilesDirectory, "stonoga.wav"));
            var sourceFile = CreateFormFileFromPath(sourceFilePath);

            var targetFilePath = GetTestProjectAbsoluteFilePath(Path.Combine(_testFilesDirectory, "zebrowski.wav"));
            var targetFile = CreateFormFileFromPath(targetFilePath);

            var model = new AudioProcessingModel { ModelName = _conversionModel, SourceFile = sourceFile, TargetFile = targetFile };

            // Act
            var result = await _audioProcessingService.ConvertVoiceAsync(model);
            var fileInfo = new FileInfo(Path.Join("wwwroot", result.OutputFilePath));

            // Assert
            Assert.True(fileInfo.Length > 0);
        }

        [Fact]
        public async Task Multimedia_Folder_Should_Be_Empty_After_Synthesis_Completes() // Except the .gitkeep file
        {
            // Arrange
            var textPrompt = "Witaj przyjacielu.";
            var targetFilePath = GetTestProjectAbsoluteFilePath(Path.Combine(_testFilesDirectory, "zebrowski.wav"));
            var targetFile = CreateFormFileFromPath(targetFilePath);
            var model = new AudioProcessingModel { ModelName = _synthesisModel, TextPrompt = textPrompt, TargetFile = targetFile };

            // Act
            var result = await _audioProcessingService.SynthesizeVoiceAsync(model);

            // Assert
            var outputFilesExist = Directory.GetFiles(_tempFilesDirectory).Any(file => !file.EndsWith(".gitkeep"));
            Assert.False(outputFilesExist, "Output folder is not empty.");
        }

        [Fact]
        public async Task Multimedia_Folder_Should_Be_Empty_After_Conversion_Completes() // Except the .gitkeep file
        {
            // Arrange
            var sourceFilePath = GetTestProjectAbsoluteFilePath(Path.Combine(_testFilesDirectory, "stonoga.wav"));
            var sourceFile = CreateFormFileFromPath(sourceFilePath);

            var targetFilePath = GetTestProjectAbsoluteFilePath(Path.Combine(_testFilesDirectory, "zebrowski.wav"));
            var targetFile = CreateFormFileFromPath(targetFilePath);

            var model = new AudioProcessingModel { ModelName = _conversionModel, SourceFile = sourceFile, TargetFile = targetFile };

            // Act
            var result = await _audioProcessingService.ConvertVoiceAsync(model);

            // Assert
            var outputFilesExist = Directory.GetFiles(_tempFilesDirectory).Any(file => !file.EndsWith(".gitkeep"));
            Assert.False(outputFilesExist, "Output folder is not empty.");
        }

        private static IFormFile CreateFormFileFromPath(string filePath)
        {
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            fileStream.Close();
            memoryStream.Position = 0;

            return new FormFile(memoryStream, 0, memoryStream.Length, "file", Path.GetFileName(filePath))
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream"
            };
        }

        private static string GetTestProjectAbsoluteFilePath(string relativePath)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var testProjectDirectory = Directory.GetParent(currentDirectory).Parent.Parent.FullName;

            return Path.Combine(testProjectDirectory, relativePath);
        }
    }
}