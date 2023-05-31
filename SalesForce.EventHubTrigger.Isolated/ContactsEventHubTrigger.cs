using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SalesForce.EventHubTrigger.Isolated.Service.Interface;

namespace SalesForce.EventHubTrigger.Isolated
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

        [Function("Function1")]
        public async Task Run([EventHubTrigger("salesforce", Connection = "conn")] string[] events, ILogger log)
        {
           
            foreach (string eventData in events)
            {
                await _accountService.CreateAccounts(eventData);
            }
        }
        
    }
}
