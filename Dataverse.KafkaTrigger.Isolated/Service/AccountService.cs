using Dataverse.KafkaTrigger.Isolated.Service.Interface;
using Dataverse.KafkaTrigger.Isolated.Model;
using Newtonsoft.Json;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;


namespace Dataverse.KafkaTrigger.Isolated.Service
{
    public class AccountService : IAccountService
    {


        public async Task CreateAccounts(string events) {

            //insert records in d365
            var SecretValue = Environment.GetEnvironmentVariable("SecretValue");
            var AppID = Environment.GetEnvironmentVariable("AppID");
            var InstanceUri = Environment.GetEnvironmentVariable("InstanceUri");
            var aid = new Guid();

            string ConnectionStr = $@"AuthType=ClientSecret;
                        SkipDiscovery=true;url={InstanceUri};
                        Secret={SecretValue};
                        ClientId={AppID};
                        RequireNewInstance=true";

            var exceptions = new List<Exception>();

           
                try
                {
                    var model = JsonConvert.DeserializeObject<Account>(events);

                    //messageReceiver.CompleteAsync(message.SystemProperties.LockToken);

                    using (ServiceClient svc = new ServiceClient(ConnectionStr))
                    {
                        if (svc.IsReady)
                        {
                            var account = new Entity("account");
                            account.Attributes["name"] = model?.name;
                            account.Attributes["accountnumber"] = model?.accountnumber;
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
