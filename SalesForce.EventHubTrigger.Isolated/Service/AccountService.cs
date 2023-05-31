using SalesForce.EventHubTrigger.Isolated.Service.Interface;
using SalesForce.EventHubTrigger.Isolated.Model;


using Microsoft.Extensions.Logging;
using Azure.Messaging.EventHubs;
using System.Text.Json;
using System.Threading.Tasks;
using Salesforce.Force;
using Salesforce.Common;
using Newtonsoft.Json;

namespace SalesForce.EventHubTrigger.Isolated.Service
{
    public class AccountService : IAccountService
    {


        public async Task CreateAccounts(string events)
        {

            //insert records in d365
            var ConsumerKey = Environment.GetEnvironmentVariable("ConsumerKey");
            var ConsumerSecret = Environment.GetEnvironmentVariable("ConsumerSecret");
            var mUsername = Environment.GetEnvironmentVariable("mUsername");
            var Password = Environment.GetEnvironmentVariable("Password");
            var SecurityToken = Environment.GetEnvironmentVariable("SecurityToken");
            var stressDelay = Environment.GetEnvironmentVariable("stressDelay");


            var exceptions = new List<Exception>();

            try
            {
                // Create a Salesforce client instance
                var auth = new AuthenticationClient();
                await auth.UsernamePasswordAsync(ConsumerKey, ConsumerSecret, mUsername, Password + SecurityToken);

                var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);

                // Define the object and fields you want to write to
                string objectName = "Account";

                var fields = JsonConvert.DeserializeObject<Account>(events);

                // Create the object record
                var response = await client.CreateAsync(objectName, fields);

                if (int.TryParse(stressDelay, out int numericValue))
                {
                    Thread.Sleep(numericValue);
                }

                await Task.Yield();
            }
            catch (Exception e)
            {
                // We need to keep processing the rest of the batch - capture this exception and continue.
                // Also, consider capturing details of the message that failed processing so it can be processed again later.
                exceptions.Add(e);
            }


            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();

        }



    }
}
