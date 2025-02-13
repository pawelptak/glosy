using System.Diagnostics;
using Glosy.Interfaces;
using Glosy.Models;
using Microsoft.AspNetCore.Mvc;

namespace Glosy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAudioProcessingService _audioProcessingService;

        public HomeController(ILogger<HomeController> logger, IAudioProcessingService audioProcessingService)
        {
            _logger = logger;
            _audioProcessingService = audioProcessingService;
        }

        public IActionResult Index()
        {
            string version = Environment.GetEnvironmentVariable("APP_VERSION") ?? "Development";
            ViewBag.Version = version;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<ActionResult> ProcessVoice(AudioProcessingModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).Distinct();

                return Json(new { error = string.Join(" ", errors) ?? "OMG" });
            }

            if (!string.IsNullOrWhiteSpace(model.TextPrompt))
            {
                var synthesisResult = await _audioProcessingService.SynthesizeVoiceAsync(model);

                if (!synthesisResult.IsSuccessful)
                {
                    return Json(new { error = synthesisResult.ErrorMessage });
                }

                return Json(new { audioUrl = synthesisResult.OutputFilePath });
            }

            var conversionResult = await _audioProcessingService.ConvertVoiceAsync(model);

            if (!conversionResult.IsSuccessful)
            {
                return Json(new { error = conversionResult.ErrorMessage });
            }

            return Json(new { audioUrl = conversionResult.OutputFilePath });
        }

        [HttpGet]
        public async Task<ActionResult> DownloadResult(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl) || !System.IO.File.Exists(fileUrl))
            {
                return NotFound("File not found.");
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(fileUrl, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;

            return File(memory, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(fileUrl));
        }
    }
}
