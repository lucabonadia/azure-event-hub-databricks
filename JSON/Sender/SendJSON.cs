using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using model;
using Nancy.Json;

namespace TestJSON
{
    public class SendJSON
    {

        static async Task Main()
        {

            string projectRoot = Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
            XDocument envoirimentalVarDoc = XDocument.Load(projectRoot + "\\..\\..\\..\\..\\EnvironmentalVariables\\environmental_vars.xml");

            // Set up namespace
            string ehubNamespaceConnectionString = envoirimentalVarDoc.Descendants("namespace-connection-string").First().Value;
            // Select Event hub
            string eventHubName = envoirimentalVarDoc.Descendants("event-hub-name").First().Value;

            await using (var producerClient = new EventHubProducerClient(ehubNamespaceConnectionString, eventHubName))
            {
                // Create a batch of events 
                using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();
                
                DateTime dateTime = DateTime.ParseExact("2000-01-01", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                UserLog userLog;
                var json = new JavaScriptSerializer();

                for (int i = 0; i < 10; i++) {
                    userLog = new UserLog("Luca", "H001", "low", dateTime.AddYears(1));
                    eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(json.Serialize(userLog))));
                }

                await producerClient.SendAsync(eventBatch);
            }
        }
    }
}
