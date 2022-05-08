using ContactSample.Models;
using ContactSample.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ContactSample.Controllers
{
    public class ContactController : Controller
    {
        private readonly ILogger<ContactController> _logger;
        private IConfiguration _configuration;
        private ContactQueueService _contactQueueService;
        private ContactService _contactService;

        public ContactController(ILogger<ContactController> logger, IConfiguration configuration, ContactQueueService contactQueueService, ContactService contactService)
        {
            _logger = logger;
            _configuration = configuration;
            _contactQueueService = contactQueueService;
            _contactService = contactService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> PostAsync([Bind("Name,Email,Phone,Comments")] ContactFormModel model)
        {
            model.IP = Common.ResolveIPAddress(HttpContext);

            //await _contactQueueService.AddAsync(model, _configuration["SendMailQueueUrl"]);
            await _contactService.AddAsync(model);

            _logger.LogInformation($"Contact added to db and id to queue. {model.LogSerialized}");

            return RedirectToAction("Index");
        }
        
    }
}
