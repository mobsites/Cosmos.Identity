// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    public class CosmosIdentityContainer : ICosmosIdentityContainer
    {
        public CosmosIdentityContainer(IConfiguration configuration)
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

            var cosmosClient = new CosmosClient(
                connection,
                new CosmosClientOptions
                {
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        IgnoreNullValues = false
                    }
                });

            Database database = cosmosClient.CreateDatabaseIfNotExistsAsync(configuration["IdentityDatabaseId"]).Result;

            IdentityContainer = database.CreateContainerIfNotExistsAsync(configuration["IdentityContainerId"], "/PartitionKey").Result;
        }

        public Container IdentityContainer { get; }
    }
}
