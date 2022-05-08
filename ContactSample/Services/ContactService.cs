using Amazon.SQS;
using Amazon.SQS.Model;
using ContactSample.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ContactSample.Services
{
    public class ContactService
    {
        private readonly ILogger _logger;
        private readonly ContactDbService _contactDbService;
        private readonly ContactQueueService _contactQueueService;
        private IConfiguration _configuration;

        public ContactService(ILogger<ContactService> logger,
            ContactDbService contactDbService, 
            ContactQueueService contactQueueService,
            IConfiguration configuration)
        {
           
            _logger = logger;
            _contactDbService = contactDbService;
            _contactQueueService = contactQueueService;
            _configuration = configuration;
        }

        public async Task<bool> AddAsync(ContactFormModel contactForm)
        {
            var id = Guid.NewGuid().ToString();
            _ = _contactDbService.AddAsync(contactForm, id);
            await _contactQueueService.AddAsync(id, _configuration["SendMailQueueUrl"]);
            return true;


            //var sendRequest = new SendMessageRequest
            //{
            //    QueueUrl = sendMailQueueUrl,
            //    MessageBody = $"{{ 'ContactForm ' : { JsonConvert.SerializeObject(contactForm) } }}"
            //};

            //var response = await _sqsClient.SendMessageAsync(sendRequest);

            //return response.HttpStatusCode == System.Net.HttpStatusCode.OK; 
        }
    }
}
