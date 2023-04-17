using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

class Program
{


    static async Task Main(string[] args)
    {

        const int numOfMessages = 10;

        bool endApp = false;
        // Display title as the C# console calculator app.
        Console.WriteLine("Console Calculator in C#\r");
        Console.WriteLine("------------------------\n");

        while (!endApp)
        {

            // create a batch 
            EventHubProducerClient producerClient = new EventHubProducerClient("Endpoint=sb://eh-blucas-test.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=hBI6MUVcQlA4F+C6Ip579BiAGsMHcPoFwg1FMzBveDE=;EntityPath=contacts");



            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            for (int i = 1; i <= numOfMessages; i++)
            {
                var account = new account
                {
                    name = "hubbatchtestddd" + i,
                    accountnumber = "hubbatchtestddd" + i
                };

                string jsonString = JsonSerializer.Serialize(account);

                // try adding a message to the batch
                if (!eventBatch.TryAdd(new EventData(jsonString)))
                {
                    // if it is too large for the batch
                    throw new Exception($"Event {i} is too large for the batch and cannot be sent.");
                }
                else { }


            }

            try
            {
                await producerClient.SendAsync(eventBatch);


            }
            catch (Exception e)
            {
                Console.WriteLine("Oh no! An exception occurred trying to do the math.\n - Details: " + e.Message);
            }
            finally
            {
                Console.WriteLine("end of run");
                await producerClient.DisposeAsync();
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