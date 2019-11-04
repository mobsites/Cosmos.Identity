using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Mobsites.AspNetCore.Identity.Cosmos;
using System;

namespace Cosmos.Identity.Default.Razor.Sample.Services
{
    public class CosmosDb : ICosmosIdentityContainer
    {
        public CosmosDb(IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
            {
                throw new Exception("No connection string.");
            }


            if (string.IsNullOrWhiteSpace(configuration["DatabaseId"]))
            {
                throw new Exception("No database id.");
            }


            if (string.IsNullOrWhiteSpace(configuration["ContainerId"]))
            {
                throw new Exception("No container id.");
            }

            var cosmosClient = new CosmosClient(
                connection,
                new CosmosClientOptions
                {
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        IgnoreNullValues = false
                    }
                });

            Database database = cosmosClient.CreateDatabaseIfNotExistsAsync(configuration["DatabaseId"]).Result;

            // Delete the existing container to prevent create identity model conflicts in sample.
            // Only for sample purposes, obviously.
            using (database.GetContainer(configuration["ContainerId"]).DeleteContainerStreamAsync().Result) { }

            IdentityContainer = database.CreateContainerIfNotExistsAsync(configuration["ContainerId"], "/PartitionKey").Result;
        }

        public Container IdentityContainer { get; }
    }
}
