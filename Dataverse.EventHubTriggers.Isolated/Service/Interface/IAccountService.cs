using Azure.Messaging.EventHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataverse.EventHubTriggers.Isolated.Service.Interface
{
    public interface IAccountService
    {
        public Task CreateAccounts(EventData events);
    }
}
