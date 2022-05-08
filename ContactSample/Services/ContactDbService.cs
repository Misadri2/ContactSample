using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using ContactSample.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ContactSample.Services
{
    public class ContactDbService
    {
        private readonly string _contactTableName;
        private readonly IConfiguration _configuration;
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly ILogger _logger;

        public ContactDbService(IConfiguration configuration, 
            IAmazonDynamoDB amazonDynamoDB, 
            ILogger<ContactDbService> logger)
        {
            _contactTableName = _configuration["ContactTableName"];
            _configuration = configuration;
            _dynamoDbClient = amazonDynamoDB;
            _logger = logger;
        }

        public async Task<bool> AddAsync(ContactFormModel contactForm, string Id)
        {
            var request = new PutItemRequest
            {
                TableName = _contactTableName,
                Item = new Dictionary<string, AttributeValue>()
                {
                    { "ID", new AttributeValue { S= Id}},
                    { "IP", new AttributeValue { S= contactForm.IP}},
                    { "Name", new AttributeValue { S= contactForm.Name}},
                    { "Email", new AttributeValue { S= contactForm.Email}},
                    { "Phone", new AttributeValue { S= contactForm.Phone}},
                    { "Comments", new AttributeValue { S= contactForm.Comments}},
                    { "DateTimeUTC", new AttributeValue { S = DateTime.UtcNow.ToString("o")}}
                }
            };

            var response =  await _dynamoDbClient.PutItemAsync(request);

            if(response.HttpStatusCode != HttpStatusCode.OK)
            {
                _logger.LogWarning($"Contact failed to save. Response details: {response.ResponseMetadata}");
            }
            return response.HttpStatusCode == HttpStatusCode.OK;
        }

    }
}
