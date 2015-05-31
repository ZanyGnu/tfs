
namespace TfsApp
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Framework.Client;
    using Microsoft.TeamFoundation.Framework.Common;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    class Program
    {
        static void Main(string[] args)
        {
            TfsConfigurationServer configurationServer =
                TfsConfigurationServerFactory.GetConfigurationServer(new Uri("http://vstfrd:8080/"));

            foreach (var collection in GetCollections(configurationServer))
            {
                Console.WriteLine("{0} {1}", collection.Key, collection.Value);
            }

            WorkItemStore workItemStore = configurationServer.GetService<WorkItemStore>();

            var collectionUri = new Uri("http://vstfrd:8080/Azure/");
            var projectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(collectionUri);
            workItemStore = projectCollection.GetService<WorkItemStore>();

            Console.WriteLine("Items created by me today:");
            WorkItemCollection workItemCollection = workItemStore.Query(
                 " SELECT [System.Id], [System.WorkItemType]," +
                 " [System.State], [System.AssignedTo], [System.Title] " +
                 " FROM WorkItems " +
                " WHERE [System.TeamProject] = 'RD'" +
                " AND [System.CreatedDate] > @Today" +
                " AND [System.CreatedBy] = 'Ajay Martin Mani' " +
                " ORDER BY [System.WorkItemType], [System.Id]");

            foreach (WorkItem wi in workItemCollection)
            {
                Console.WriteLine("{0}, {1}, {2}, {3}", wi.Id, wi.AreaPath, wi.IterationPath, wi.Title);
            }

            var project = workItemStore.Projects["RD"];
            QueryHierarchy queryHierarchy = project.QueryHierarchy;
            var queryFolder = queryHierarchy as QueryFolder;
            var queryItem = ((queryFolder["Shared Queries"] as QueryFolder)["Aztec"] as QueryFolder)["Compute - Aztec - 5.2 - Backlog"] as QueryItem;

            // following line doesnt work
            // workItemCollection = workItemStore.Query(((QueryDefinition)queryItem).QueryText);
            foreach (WorkItem wi in workItemCollection)
            {
                Console.WriteLine("{0}, {1}, {2}, {3}", wi.Id, wi.AreaPath, wi.IterationPath, wi.Title);
            }
        }

        public static IList<KeyValuePair<Guid, String>> GetCollections(TfsConfigurationServer configurationServer)
        {
            //ApplicationLogger.Log("Entered into GetCollections() : ");
            var collectionList = new List<KeyValuePair<Guid, String>>();
            try
            {
                configurationServer.Authenticate();

                ReadOnlyCollection<CatalogNode> collectionNodes = configurationServer.CatalogNode.QueryChildren(
                    new[] { CatalogResourceTypes.ProjectCollection },
                    false,
                    CatalogQueryOptions.None);
                foreach (CatalogNode collectionNode in collectionNodes)
                {
                    var collectionId = new Guid(collectionNode.Resource.Properties["InstanceId"]);
                    TfsTeamProjectCollection teamProjectCollection =
                        configurationServer.GetTeamProjectCollection(collectionId);

                    if (teamProjectCollection == null)
                        continue;

                    collectionList.Add(new KeyValuePair<Guid, String>(collectionId, teamProjectCollection.Name));
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }

            return collectionList;
        }
    }
}
