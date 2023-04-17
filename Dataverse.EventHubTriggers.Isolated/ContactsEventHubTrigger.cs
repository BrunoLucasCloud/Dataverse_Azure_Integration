using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Dataverse.EventHubTriggers.Isolated
{
    public class ContactsEventHubTrigger
    {
        private readonly ILogger _logger;

        public ContactsEventHubTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ContactsEventHubTrigger>();
        }

        [Function("PullFromContactEntity")]
        public void Run([EventHubTrigger("samples-workitems", Connection = "")] string[] input)
        {
            _logger.LogInformation($"First Event Hubs triggered message: {input[0]}");
        }
    }
}
