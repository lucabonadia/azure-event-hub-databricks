using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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


        }

    }

}
