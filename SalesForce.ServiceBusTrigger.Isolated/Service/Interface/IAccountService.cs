using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForce.ServiceBusTrigger.Isolated.Service.Interface
{
    public interface IAccountService
    {
        public Task CreateAccounts(string events);
    }
}
