using System.Diagnostics;
using System.IO;
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
            FileStream fileStream = null;

            if (model.SourceFile != null)
            {
                fileStream = await _audioProcessingService.ConvertVoiceAsync(model);
            }
            else if (!string.IsNullOrWhiteSpace(model.TextPrompt))
            {
                fileStream = await _audioProcessingService.SynthesizeVoiceAsync(model);
            }

            return File(fileStream, System.Net.Mime.MediaTypeNames.Application.Octet, "out.wav");
        }
    }
}
