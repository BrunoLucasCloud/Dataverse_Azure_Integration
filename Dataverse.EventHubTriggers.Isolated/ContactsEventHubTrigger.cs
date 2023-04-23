using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Dataverse.EventHubTriggers.Isolated.Service.Interface;

namespace Dataverse.EventHubTriggers.Isolated
{
    public class ContactsEventHubTrigger
    {
        private readonly ILogger _logger;
        private readonly IAccountService _accountService;

        public ContactsEventHubTrigger(ILoggerFactory loggerFactory, IAccountService accountService)
        {
            _logger = loggerFactory.CreateLogger<ContactsEventHubTrigger>();
            _accountService = accountService;
        }

        [Function("PullFromContactEntity")]
        public async Task Run([EventHubTrigger("contacts", Connection = "conn")] string[] events, ILogger log)
        {
            foreach (string eventData in events)
            {
                await _accountService.CreateAccounts(eventData);
            }            
        }
    }
}
