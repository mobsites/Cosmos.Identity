// Copyright (c) 2020 Allan Mobley. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.Cosmos;

namespace Mobsites.Cosmos.Identity
{
    /// <summary>
    ///     Options for configuring the Cosmos storage provider. 
    /// </summary>
    public class CosmosStorageProviderOptions
    {
        /// <summary>
        ///     Gets or sets the connection string to the Azure Cosmos DB service. 
        ///     Defaults to the default Azure Cosmos DB Emulator connection string if not set.
        /// </summary>
        /// <remarks>
        ///     If you are using the emulator for testing and have started the emulator with the /Key option, then pass in the generated key instead.
        /// </remarks>
        public string ConnectionString { get; set; } = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";


        /// <summary>
        ///     Gets or sets the options for configuring the client used by Cosmos Identity to access the Azure Cosmos DB service. 
        /// </summary>
        public CosmosClientOptions CosmosClientOptions { get; set; }


        /// <summary>
        ///     Gets or sets the id of the database to be used by Cosmos Identity in the Azure Cosmos DB service. 
        ///     Defaults to "IdentityDatabase" if not set.
        /// </summary>
        /// <remarks>
        ///     If the database does not exist, it will be created.
        /// </remarks>
        public string DatabaseId { get; set; } = "IdentityDatabase";


        /// <summary>
        ///     Gets or sets a set of additional options that can be set for the database. 
        /// </summary>
        public RequestOptions DatabaseRequestOptions { get; set; }


        /// <summary>
        ///     The throughput provisioned for a database in measurement of Request Units per second in the Azure Cosmos DB service. 
        /// </summary>
        public int? DatabaseThroughput { get; set; }


        /// <summary>
        ///     Gets or sets the properties for the container to be used by Cosmos Identity in the Azure Cosmos DB service. 
        ///     Defaults to { Id = "IdentityContainer", PartitionKeyPath = "/PartitionKey" } if not set.
        /// </summary>
        /// <remarks>
        ///     If the container does not exist, it will be created.
        /// </remarks>
        public ContainerProperties ContainerProperties { get; set; }


        /// <summary>
        ///     Gets or sets a set of additional options that can be set for the container. 
        /// </summary>
        public RequestOptions ContainerRequestOptions { get; set; }


        /// <summary>
        ///     The throughput provisioned for a container in measurement of Request Units per second in the Azure Cosmos DB service. 
        /// </summary>
        public int? ContainerThroughput { get; set; }
    }
}
