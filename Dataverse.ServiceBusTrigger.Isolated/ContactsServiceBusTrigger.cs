using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Dataverse.ServiceBusTrigger.Isolated.Service.Interface;
using System.ServiceModel.Channels;

namespace Dataverse.ServiceBusTrigger.Isolated
{
    public class ContactsServiceBusTrigger
    {
        private readonly ILogger _logger;
        private readonly IAccountService _accountService;

        public ContactsServiceBusTrigger(ILoggerFactory loggerFactory, IAccountService accountService)
        {
            _logger = loggerFactory.CreateLogger<ContactsServiceBusTrigger>();
            _accountService = accountService;
        }

        [Function("Function1")]
        public async Task Run([ServiceBusTrigger("accounts", "dataverse", Connection = "conn", IsBatched= true)] string[] mySbMsg)
        {
            foreach (string msg in mySbMsg)
            {                
                await _accountService.CreateAccounts(msg);
            }
        }
    }
}
