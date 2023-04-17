
using System;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Text.Json.Serialization;
class Program
{


    static async Task Main(string[] args)
    {
        ServiceBusClient client;
        ServiceBusSender sender;
        const int numOfMessages = 3000;

        bool endApp = false;
        // Display title as the C# console calculator app.
        Console.WriteLine("Console Calculator in C#\r");
        Console.WriteLine("------------------------\n");

        while (!endApp)
        {

            var clientOptions = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            client = new ServiceBusClient("Endpoint=sb://sb-testforduplicate-2023.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=MCGj+GMxAOXBJfVUCNYBs81YuWmbGiAhQ+ASbHjXgJU=;EntityPath=accounts");
            sender = client.CreateSender("accounts");

            // create a batch 
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            for (int i = 1; i <= numOfMessages; i++)
            {
                var account = new account
                {
                    name = "xxxe" + i,
                    accountnumber = "yyyye" + i
                };

                string jsonString = JsonSerializer.Serialize(account);

                // try adding a message to the batch
                if (!messageBatch.TryAddMessage(new ServiceBusMessage(jsonString)))
                {
                    // if it is too large for the batch
                    throw new Exception($"The message {i} is too large to fit in the batch.");
                }
            }

            try
            {
                await sender.SendMessagesAsync(messageBatch);
            }
            catch (Exception e)
            {
                Console.WriteLine("Oh no! An exception occurred trying to do the math.\n - Details: " + e.Message);
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }

            Console.WriteLine("------------------------\n");

            // Wait for the user to respond before closing.
            Console.Write("Press 'n' and Enter to close the app, or press any other key and Enter to continue: ");
            if (Console.ReadLine() == "n") endApp = true;

            Console.WriteLine("\n"); // Friendly linespacing.
        }
        return;
    }
}

class account
{

    public string name { get; set; }

    public string accountnumber { get; set; }


}