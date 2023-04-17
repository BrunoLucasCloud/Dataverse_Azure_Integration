using Dataverse.EventHubTriggers.Isolated.Service.Interface;
using Dataverse.EventHubTriggers.Isolated.Model;
using Newtonsoft.Json;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventHubs;

namespace Dataverse.EventHubTriggers.Isolated.Service
{
    public class AccountService : IAccountService
    {


        public async Task CreateAccounts(EventData events) {

            //insert records in d365
            var SecretValue = Environment.GetEnvironmentVariable("SecretValue");
            var AppID = Environment.GetEnvironmentVariable("AppID");
            var InstanceUri = Environment.GetEnvironmentVariable("InstanceUri");
            var aid = new Guid();

            string ConnectionStr = $@"AuthType=ClientSecret;
                        SkipDiscovery=true;url={"https://orgac1b9f65.crm6.dynamics.com/"};
                        Secret={"Izn8Q~AvbAO7WxfESoiedyUcOyCmgpRhkX0mHaSq"};
                        ClientId={"ee555fa0-9b4c-4a65-8962-28c48e95072f"};
                        RequireNewInstance=true";

            var exceptions = new List<Exception>();

           
                try
                {
                    var model = JsonConvert.DeserializeObject<Account>(events.EventBody.ToString());

                    //messageReceiver.CompleteAsync(message.SystemProperties.LockToken);

                    using (ServiceClient svc = new ServiceClient(ConnectionStr))
                    {
                        if (svc.IsReady)
                        {
                            var account = new Entity("account");
                            account.Attributes["name"] = "model.name";
                            account.Attributes["accountnumber"] = "model.accountnumber";
                            aid = await svc.CreateAsync(account);
                        }
                    }

                    //log.LogInformation($"C# Event Hub trigger function processed a message: {eventData.CorrelationId}");

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
