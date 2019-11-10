// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     Represents a new instance of a default Cosmos storage provider which implements <see cref="IIdentityStorageProvider"/>.
    /// </summary>
    public class CosmosStorageProvider : IIdentityStorageProvider
    {
        #region Setup

        private readonly CosmosClient cosmosClient;
        private readonly Database database;

        public Container Container { get; set; }

        /// <summary>
        ///     Constructs a new instance of <see cref="CosmosStorageProvider"/>.
        /// </summary>
        /// <param name="configuration">The application configuration settings.</param>
        public CosmosStorageProvider(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("CosmosStorageProvider");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("No connection string.");
            }
            var databaseId = configuration["CosmosStorageProviderDatabaseId"];
            if (string.IsNullOrWhiteSpace(databaseId))
            {
                throw new Exception("No database id.");
            }
            var containerId = configuration["CosmosStorageProviderContainerId"];
            if (string.IsNullOrWhiteSpace(containerId))
            {
                throw new Exception("No container id.");
            }
            var partitionKeyPath = configuration["CosmosStorageProviderPartitionKeyPath"];
            if (string.IsNullOrWhiteSpace(partitionKeyPath))
            {
                // Defaults to known-property "PartitionKey" on base Identity models under namespace Mobsites.AspNetCore.Identity.Cosmos.
                partitionKeyPath = "/PartitionKey";
            }
            else
            {
                if (!partitionKeyPath.StartsWith("/"))
                {
                    partitionKeyPath = "/" + partitionKeyPath;
                }
            }

            cosmosClient = new CosmosClient(
                connectionString,
                new CosmosClientOptions
                {
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        IgnoreNullValues = false
                    }
                });

            database = cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId).Result;

            Container = database.CreateContainerIfNotExistsAsync(containerId, partitionKeyPath).Result;
        }

        #endregion

        #region Queryable Linq Expression

        /// <summary>
        ///     Returns a queryable linq expression of the specified <typeparamref name="TCosmosStorageType" />.
        /// </summary>
        public IOrderedQueryable<TCosmosStorageType> Queryable<TCosmosStorageType>()
            where TCosmosStorageType : ICosmosStorageType, new()
        {
            var partitionKey = new TCosmosStorageType().PartitionKey;

            return Container.GetItemLinqQueryable<TCosmosStorageType>(
                allowSynchronousQueryExecution:true,
                requestOptions: new QueryRequestOptions
                {
                    PartitionKey = string.IsNullOrEmpty(partitionKey) ? PartitionKey.None : new PartitionKey(partitionKey)
                });
        }

        #endregion

        #region CreateAsync

        /// <summary>
        ///     Creates the specified <paramref name="cosmosStorageType"/> in the store.
        /// </summary>
        /// <param name="cosmosStorageType">The storage type to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.
        /// </returns>
        public async Task<IdentityResult> CreateAsync<TCosmosStorageType>(TCosmosStorageType cosmosStorageType, CancellationToken cancellationToken = default)
            where TCosmosStorageType : ICosmosStorageType, new()
        {
            IdentityResult result;

            cancellationToken.ThrowIfCancellationRequested();

            if (cosmosStorageType == null)
            {
                result = IdentityResult.Failed(new IdentityError() { Code = HttpStatusCode.BadRequest.ToString(), Description = $"Argument {nameof(cosmosStorageType)} cannot be null." });
            }
            else
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(cosmosStorageType.PartitionKey) ? PartitionKey.None : new PartitionKey(cosmosStorageType.PartitionKey);

                    var response = await Container.CreateItemAsync(cosmosStorageType, partitionKey, cancellationToken: cancellationToken);

                    result = response.StatusCode >= HttpStatusCode.BadRequest ?
                        IdentityResult.Failed(new IdentityError() { Code = response.StatusCode.ToString(), Description = $"The storage type {cosmosStorageType.GetType().Name} was not created." }) :
                        IdentityResult.Success;
                }
                catch (CosmosException ex)
                {
                    result = IdentityResult.Failed(new IdentityError() { Code = ex.StatusCode.ToString(), Description = ex.Message });
                }
            }

            return result;
        }

        #endregion

        #region UpdateAsync

        /// <summary>
        ///     Updates the specified <paramref name="cosmosStorageType"/> in the store.
        /// </summary>
        /// <param name="cosmosStorageType">The storage type to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.
        /// </returns>
        public async Task<IdentityResult> UpdateAsync<TCosmosStorageType>(TCosmosStorageType cosmosStorageType, CancellationToken cancellationToken = default)
            where TCosmosStorageType : ICosmosStorageType, new()
        {
            IdentityResult result;

            cancellationToken.ThrowIfCancellationRequested();

            if (cosmosStorageType == null)
            {
                result = IdentityResult.Failed(new IdentityError() { Code = HttpStatusCode.BadRequest.ToString(), Description = $"Argument {nameof(cosmosStorageType)} cannot be null." });
            }
            else
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(cosmosStorageType.PartitionKey) ? PartitionKey.None : new PartitionKey(cosmosStorageType.PartitionKey);

                    var response = await Container.ReplaceItemAsync(cosmosStorageType, cosmosStorageType.Id, partitionKey, cancellationToken: cancellationToken);

                    result = response.StatusCode >= HttpStatusCode.BadRequest ?
                        IdentityResult.Failed(new IdentityError() { Code = response.StatusCode.ToString(), Description = $"The storage type {cosmosStorageType.GetType().Name} was not updated." }) :
                        IdentityResult.Success;
                }
                catch (CosmosException ex)
                {
                    result = IdentityResult.Failed(new IdentityError() { Code = ex.StatusCode.ToString(), Description = ex.Message });
                }
            }

            return result;
        }

        #endregion

        #region DeleteAsync

        /// <summary>
        ///     Deletes the specified <paramref name="cosmosStorageType"/> from the store.
        /// </summary>
        /// <param name="cosmosStorageType">The storage type to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the delete operation.
        /// </returns>
        public async Task<IdentityResult> DeleteAsync<TCosmosStorageType>(TCosmosStorageType cosmosStorageType, CancellationToken cancellationToken = default)
            where TCosmosStorageType : ICosmosStorageType, new()
        {
            IdentityResult result;

            cancellationToken.ThrowIfCancellationRequested();

            if (cosmosStorageType == null)
            {
                result = IdentityResult.Failed(new IdentityError() { Code = HttpStatusCode.BadRequest.ToString(), Description = $"Argument {nameof(cosmosStorageType)} cannot be null." });
            }
            else
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(cosmosStorageType.PartitionKey) ? PartitionKey.None : new PartitionKey(cosmosStorageType.PartitionKey);

                    var response = await Container.DeleteItemAsync<TCosmosStorageType>(cosmosStorageType.Id, partitionKey, cancellationToken: cancellationToken);

                    result = response.StatusCode >= HttpStatusCode.BadRequest ?
                        IdentityResult.Failed(new IdentityError() { Code = response.StatusCode.ToString(), Description = $"The storage type {cosmosStorageType.GetType().Name} was not deleted." }) :
                        IdentityResult.Success;
                }
                catch (CosmosException ex)
                {
                    result = IdentityResult.Failed(new IdentityError() { Code = ex.StatusCode.ToString(), Description = ex.Message });
                }
            }

            return result;
        }

        #endregion

        #region FindByIdAsync

        /// <summary>
        ///     Finds and returns the specified <typeparamref name="TCosmosStorageType" />, if any, which has the specified <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="TCosmosStorageType">The type to find.</typeparam>
        /// <param name="id">The id of the type to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <typeparamref name="TCosmosStorageType" /> with the specified <paramref name="id"/> if it exists.
        /// </returns>
        public async Task<TCosmosStorageType> FindByIdAsync<TCosmosStorageType>(string id, CancellationToken cancellationToken = default)
            where TCosmosStorageType : ICosmosStorageType, new()
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var cosmosStorageType = new TCosmosStorageType();
                    var partitionKey = string.IsNullOrEmpty(cosmosStorageType.PartitionKey) ? PartitionKey.None : new PartitionKey(cosmosStorageType.PartitionKey);

                    return await Container.ReadItemAsync<TCosmosStorageType>(id, partitionKey, cancellationToken: cancellationToken);
                }
                catch (CosmosException)
                {

                }
            }

            return default;
        }

        #endregion
    }
}
