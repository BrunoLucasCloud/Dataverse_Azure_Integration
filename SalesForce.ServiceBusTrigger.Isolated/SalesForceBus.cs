using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SalesForce.ServiceBusTrigger.Isolated.Service.Interface;

namespace SalesForce.ServiceBusTrigger.Isolated
{
    public class SalesForceBus
    {
        private readonly ILogger _logger;
        private readonly IAccountService _accountService;

        public SalesForceBus(ILoggerFactory loggerFactory,IAccountService accountService)
        {
            _logger = loggerFactory.CreateLogger<SalesForceBus>();
            _accountService = accountService;

        }

        [Function("SalesForceBusTrigger")]
        public async Task Run([ServiceBusTrigger("accounts", "dataverse", Connection = "conn", IsBatched= true)] string[] mySbMsg)
        {
            foreach (string msg in mySbMsg)
            {
                await _accountService.CreateAccounts(msg);
            }
        }
    }
}
