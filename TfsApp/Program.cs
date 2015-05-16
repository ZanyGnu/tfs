using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;

namespace TfsApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Connect to Team Foundation Server
            //     server is the name of the server that is running the Team Foundation application-tier.
            //     port is the port that Team Foundation uses. The default is port is 8080.
            //     vpath is the virutal path to the Team Foundation application. The default path is tfs.
            TfsConfigurationServer configurationServer =
                TfsConfigurationServerFactory.GetConfigurationServer(new Uri("http://vstfrd:8080/"));

            // Get the catalog of team project collections
            CatalogNode catalogNode = configurationServer.CatalogNode;
            ReadOnlyCollection<CatalogNode> tpcNodes = catalogNode.QueryChildren(
                new Guid[] { CatalogResourceTypes.ProjectCollection },
                false, CatalogQueryOptions.None);

            // List the team project collections
            foreach (CatalogNode tpcNode in tpcNodes)
            {
                // Use the InstanceId property to get the team project collection
                Guid tpcId = new Guid(tpcNode.Resource.Properties["InstanceId"]);
                TfsTeamProjectCollection tpc = configurationServer.GetTeamProjectCollection(tpcId);

                // Print the name of the team project collection
                Console.WriteLine("Collection: " + tpc.Name);

                // Get catalog of team projects for the collection
                ReadOnlyCollection<CatalogNode> tpNodes = tpcNode.QueryChildren(
                    new Guid[] { CatalogResourceTypes.TeamProject },
                    false, CatalogQueryOptions.None);

                // List the team projects in the collection
                foreach (CatalogNode tpNode in tpNodes)
                {
                    Console.WriteLine("Team Project: " + tpNode.Resource.DisplayName);
                }
            }
        }
    }
}
