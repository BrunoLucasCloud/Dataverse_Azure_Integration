using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Threading;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {

        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfiguration AppSettings = builder.Build();

        var SecretValue = AppSettings["ConnectionString:SecretValue"];
        var AppID = AppSettings["ConnectionString:AppID"];
        var InstanceUri = AppSettings["ConnectionString:InstanceUri"];
        var aid = new Guid();
        var tcs = new TaskCompletionSource<string>();

        string ConnectionStr = $@"AuthType=ClientSecret;
                        SkipDiscovery=true;url={InstanceUri};
                        Secret={SecretValue};
                        ClientId={AppID};
                        RequireNewInstance=true";

        var conf = new ConsumerConfig
        {
            BootstrapServers = AppSettings["ConnectionString:BootstrapServers"], // Replace with your Kafka broker(s)
            GroupId = "$Default",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            SaslMechanism = SaslMechanism.Plain, // Use SASL/SCRAM authentication
            SaslUsername = AppSettings["ConnectionString:SaslUsername"], // Replace with your username
            SaslPassword = AppSettings["ConnectionString:SaslPassword"], // Replace with your password
            SecurityProtocol = SecurityProtocol.SaslSsl, // Use SSL encryption for security
        };

        using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
        {
            c.Subscribe(AppSettings["ConnectionString:topic"]);

            try
            {
                while (true)
                {

                    var cr = c.Consume(CancellationToken.None); // Use a cancellation token for graceful shutdown

                    var model = JsonConvert.DeserializeObject<account>(cr.Value.ToString());

                    using (ServiceClient svc = new ServiceClient(ConnectionStr))
                    {
                        if (svc.IsReady)
                        {
                            var account = new Entity("account");
                            account.Attributes["name"] = model?.name;
                            account.Attributes["accountnumber"] = model?.accountnumber;
                            aid = await svc.CreateAsync(account);
                            Console.WriteLine("successfully commited  " + model?.name);
                        }
                    }

                    c.Commit(cr);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Consumer has been cancelled.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occurred: {e.Message}");
            }
            finally
            {
                c.Close();

            }
            
        }


    }

  
    class account
    {

        public string name { get; set; }

        public string accountnumber { get; set; }


    }

}
