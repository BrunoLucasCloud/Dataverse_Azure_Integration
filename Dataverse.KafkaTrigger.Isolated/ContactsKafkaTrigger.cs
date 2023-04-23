
using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Dataverse.KafkaTrigger.Isolated.Service.Interface;

namespace Dataverse.KafkaTrigger.Isolated
{
    public class ContactsKafkaTrigger
    {
        private readonly ILogger _logger;
        private readonly IAccountService _accountService;

        public ContactsKafkaTrigger(ILoggerFactory loggerFactory, IAccountService accountService)
        {
            _logger = loggerFactory.CreateLogger<ContactsKafkaTrigger>();
            _accountService = accountService;
        }

        [Function("KafkaTrigger")]
        public async Task Run(
             [KafkaTrigger("BrokerList",
                          "topic",
                          Username = "ConfluentCloudUserName",
                          Password = "ConfluentCloudPassword",
                          Protocol = BrokerProtocol.SaslSsl,
                          AuthenticationMode = BrokerAuthenticationMode.Plain,
                          ConsumerGroup = "$Default")] string eventData, FunctionContext context)
        {
            var logger = context.GetLogger("KafkaFunction");
            logger.LogInformation($"C# Kafka trigger function processed a message: {JObject.Parse(eventData)["Value"]}");
            await _accountService.CreateAccounts(eventData);
        }
    }
}
