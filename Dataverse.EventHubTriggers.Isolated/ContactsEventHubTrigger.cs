using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Dataverse.EventHubTriggers.Isolated.Service;
using Azure.Messaging.EventHubs;

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
        public async Task Run([EventHubTrigger("samples-workitems", Connection = "")] EventData[] events, AccountService accountService)
        {

            foreach (EventData eventData in events)
            {
                await accountService.CreateAccounts(eventData);

            }

            //_logger.LogInformation($"First Event Hubs triggered message: {input[0]}");
        }
    }
}
