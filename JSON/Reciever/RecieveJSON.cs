using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using System;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using Nancy.Json;
using model;

namespace RecieverJSON
{
    class RecieveJSON
    {
        static Boolean recievedAnyEvent = false;
        static async Task Main()
        {

            string projectRoot = Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
            XDocument envoirimentalVarDoc = XDocument.Load(projectRoot + "\\..\\..\\..\\..\\EnvironmentalVariables\\environmental_vars.xml");

            // Set up namespace
            string ehubNamespaceConnectionString = envoirimentalVarDoc.Descendants("namespace-connection-string").First().Value;
            // Select Event hub
            string eventHubName = envoirimentalVarDoc.Descendants("event-hub-name").First().Value;

            string blobStorageConnectionString = envoirimentalVarDoc.Descendants("blob-storage-connection-string").First().Value;
            string blobContainerName = envoirimentalVarDoc.Descendants("blob-container-name").First().Value;

            // Read from the default consumer group: $Default
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            // Create a blob container client that the event processor will use 
            BlobContainerClient storageClient = new BlobContainerClient(blobStorageConnectionString, blobContainerName);

            // Create an event processor client to process events in the event hub
            EventProcessorClient processor = new EventProcessorClient(storageClient, consumerGroup, ehubNamespaceConnectionString, eventHubName);

            // Register handlers for processing events and handling errors
            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;

            // Start the processing
            await processor.StartProcessingAsync();

            // Wait for 10 seconds for the events to be processed
            await Task.Delay(TimeSpan.FromSeconds(100));

            if (!recievedAnyEvent)
            {
                Console.WriteLine("No events recieved.");
            }

            // Stop the processing
            await processor.StopProcessingAsync();

        }

        static Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            var userLog = new JavaScriptSerializer().Deserialize<UserLog>(Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));
            // Write the body of the event to the console window
            Console.WriteLine("\tReceived event from: {0}", userLog.ToString());
            recievedAnyEvent = true;

            // This makes you only recieve new events
            eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
            return Task.CompletedTask;
        }

        static Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            // Write details about the error to the console window
            Console.WriteLine($"\tPartition '{ eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
            Console.WriteLine(eventArgs.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
