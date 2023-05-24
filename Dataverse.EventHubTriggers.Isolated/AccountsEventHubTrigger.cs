using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Dataverse.EventHubTriggers.Isolated.Service.Interface;

namespace Dataverse.EventHubTriggers.Isolated
{
    public class AccountsEventHubTrigger
    {
        private readonly ILogger _logger;
        private readonly IAccountService _accountService;

        public AccountsEventHubTrigger(ILoggerFactory loggerFactory, IAccountService accountService)
        {
            _logger = loggerFactory.CreateLogger<AccountsEventHubTrigger>();
            _accountService = accountService;
        }

        [Function("PullFromContactEntity")]
        public async Task Run([EventHubTrigger("accounts", Connection = "conn")] string[] events, ILogger log)
        {
            foreach (string eventData in events)
            {
                await _accountService.CreateAccounts(eventData);
            }            
        }
    }
}
