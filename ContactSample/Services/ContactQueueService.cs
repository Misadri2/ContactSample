using Amazon.SQS;
using Amazon.SQS.Model;
using ContactSample.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ContactSample.Services
{
    public class ContactQueueService
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly ILogger _logger;

        public ContactQueueService(IAmazonSQS sqsClient, ILogger logger)
        {
            _sqsClient = sqsClient;
            _logger = logger;
        }

        public async Task<bool> AddAsync(ContactFormModel contactForm, string sendMailQueueUrl)
        {
            var sendRequest = new SendMessageRequest
            {
                QueueUrl = sendMailQueueUrl,
                MessageBody = $"{{ 'ContactForm ' : { JsonConvert.SerializeObject(contactForm) } }}"
            };

            var response = await _sqsClient.SendMessageAsync(sendRequest);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK; 
        }
    }
}
