using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace TestTesi
{
    class Sender
    {
        static async Task Main()
        {
            string projectRoot = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            XDocument envoirimentalVarDoc = XDocument.Load(projectRoot + "\\..\\..\\..\\EnvironmentalVariables\\environmental_vars.xml");

            // Set up namespace
            string connectionString = envoirimentalVarDoc.Descendants("namespace-connection-string").First().Value;
            // Select Event hub
            string eventHubName = envoirimentalVarDoc.Descendants("event-hub-name").First().Value;

            // Create a producer client that you can use to send events to an event hub
            await using (var producerClient = new EventHubProducerClient(connectionString, eventHubName))
            {
                // Create a batch of events 
                using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

                // Add events to the batch. An event is a represented by a collection of bytes and metadata. 
                for (int i = 0; i < 10; i++)    
                    eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("Testing, test n°"+i)));
    
                // Use the producer client to send the batch of events to the event hub
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine("A batch of 10 events has been published.");
            }
        }
    }
}
