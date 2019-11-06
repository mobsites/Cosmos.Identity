using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Mobsites.AspNetCore.Identity.Cosmos;
using System;

namespace Extended.Cosmos.Identity.Razor.Sample.Extensions
{
    public class CustomCosmosIdentityContainer : ICosmosIdentityContainer
    {
        public CustomCosmosIdentityContainer(IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("CosmosIdentity");

            if (string.IsNullOrWhiteSpace(connection))
            {
                throw new Exception("No connection string.");
            }


            if (string.IsNullOrWhiteSpace(configuration["IdentityDatabaseId"]))
            {
                throw new Exception("No database id.");
            }


            if (string.IsNullOrWhiteSpace(configuration["IdentityContainerId"]))
            {
                throw new Exception("No container id.");
            }

            // Custom configure client.
            var cosmosClient = new CosmosClient(
                connection,
                new CosmosClientOptions
                {
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        IgnoreNullValues = false
                    }
                });

            // Custom configure database if desired.
            Database database = cosmosClient.CreateDatabaseIfNotExistsAsync(configuration["IdentityDatabaseId"]).Result;

            // Custom configure database if desired.
            IdentityContainer = database.CreateContainerIfNotExistsAsync(configuration["IdentityContainerId"], configuration["IdentityPartitionKeyPath"]).Result;
        }

        public Container IdentityContainer { get; }
    }
}
