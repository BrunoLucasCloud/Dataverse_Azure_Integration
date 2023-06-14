using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace KafkaProducerExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration AppSettings = builder.Build();           

            var config = new ProducerConfig
            {
                BootstrapServers = AppSettings["ConnectionString:BootstrapServers"],
                //TransactionalId = Guid.NewGuid().ToString(), // Unique ID for the transactional producer
                SaslMechanism = SaslMechanism.Plain, // Use SASL/SCRAM authentication
                SaslUsername = AppSettings["ConnectionString:SaslUsername"], // Replace with your username
                SaslPassword = AppSettings["ConnectionString:SaslPassword"], // Replace with your password
                SecurityProtocol = SecurityProtocol.SaslSsl, // Use SSL encryption for security
                EnableIdempotence = true,
                ClientId = new Guid().ToString(),
                Acks = Acks.All,
            };

            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                //producer.InitTransactions(TimeSpan.FromSeconds(100));

                try
                {
                    int numOfMessages = Convert.ToInt32(AppSettings["ConnectionString:events_amount"]);
                    var nameval = GenerateRandomKey(10);

                    bool endApp = false;

                    while (!endApp)
                    {
                        try
                        {
                            for (int i = 1; i <= numOfMessages; i++)
                            {

                                var account = new account
                                {
                                    name = nameval + i,
                                    accountnumber = nameval + i
                                };

                                string jsonString = JsonSerializer.Serialize(account);

                                var message1 = new Message<string, string>
                                {
                                    Key = "my-key-" + i,
                                    Value = jsonString
                                };

                                //producer.BeginTransaction();
                                await producer.ProduceAsync("topic_2", message1);
                                //producer.CommitTransaction();
                                Console.WriteLine("Messages sent successfully and committed transaction." + nameval + i);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Oh no! An exception occurred trying to do the math.\n - Details: " + e.Message);
                        }
                        finally
                        {
                            Console.WriteLine("end of run");
                            //await producerClient.DisposeAsync();
                        }

                        Console.WriteLine("------------------------\n");

                        // Wait for the user to respond before closing.
                        Console.Write("Press 'n' and Enter to close the app, or press any other key and Enter to continue: ");
                        if (Console.ReadLine() == "n") endApp = true;

                        Console.WriteLine("\n"); // Friendly linespacing.
                    }


                }
                catch (ProduceException<string, string> e)
                {
                    producer.AbortTransaction();
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }



        public static string GenerateRandomKey(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var key = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                key.Append(chars[random.Next(chars.Length)]);
            }

            return key.ToString();
        }

    }

    class account
    {

        public string name { get; set; }

        public string accountnumber { get; set; }


    }

}
