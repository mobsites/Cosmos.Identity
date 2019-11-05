// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     Represents a new instance of a Cosmos identity storage provider.
    /// </summary>
    public class CosmosIdentityStorageProvider : ICosmosIdentityStorageProvider
    {
        #region Setup

        private readonly Container container;

        /// <summary>
        ///     Constructs a new instance of <see cref="CosmosIdentityStorageProvider"/>.
        /// </summary>
        /// <param name="cosmos">The context in which to access the Cosmos Container for the identity store.</param>
        public CosmosIdentityStorageProvider(ICosmosIdentityContainer cosmos)
        {
            container = cosmos?.IdentityContainer ?? throw new ArgumentNullException(nameof(cosmos));
        }

        #endregion

        #region Queryable Linq Expression

        /// <summary>
        ///     Returns a queryable linq expression of the specified <typeparam name="TIdentity"/>.
        /// </summary>
        public IOrderedQueryable<TIdentity> Queryable<TIdentity>()
            where TIdentity : ICosmosIdentity, new()
        {
            var partitionKey = new TIdentity().PartitionKey;

            return container.GetItemLinqQueryable<TIdentity>(
                allowSynchronousQueryExecution:true,
                requestOptions: new QueryRequestOptions
                {
                    PartitionKey = string.IsNullOrEmpty(partitionKey) ? PartitionKey.None : new PartitionKey(partitionKey)
                });
        }

        #endregion

        #region Create Identity Model

        /// <summary>
        ///     Creates the specified <paramref name="identityModel"/> in the store.
        /// </summary>
        /// <param name="identityModel">The identity model to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.
        /// </returns>
        public async Task<IdentityResult> CreateAsync<TIdentity>(TIdentity identityModel, CancellationToken cancellationToken = default)
            where TIdentity : ICosmosIdentity, new()
        {
            IdentityResult result;

            cancellationToken.ThrowIfCancellationRequested();

            if (identityModel == null)
            {
                result = IdentityResult.Failed(new IdentityError() { Code = HttpStatusCode.BadRequest.ToString(), Description = $"Argument cannot be null." });
            }
            else
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(identityModel.PartitionKey) ? PartitionKey.None : new PartitionKey(identityModel.PartitionKey);

                    var response = await container.CreateItemAsync(identityModel, partitionKey, cancellationToken: cancellationToken);

                    result = response.StatusCode >= HttpStatusCode.BadRequest ?
                        IdentityResult.Failed(new IdentityError() { Code = response.StatusCode.ToString(), Description = $"The identity of type {identityModel.GetType().Name} was not created." }) :
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

        #region Update Identity Model

        /// <summary>
        ///     Updates the specified <paramref name="identityModel"/> in the store.
        /// </summary>
        /// <param name="identityModel">The identity model to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.
        /// </returns>
        public async Task<IdentityResult> UpdateAsync<TIdentity>(TIdentity identityModel, CancellationToken cancellationToken = default)
            where TIdentity : ICosmosIdentity, new()
        {
            IdentityResult result;

            cancellationToken.ThrowIfCancellationRequested();

            if (identityModel == null)
            {
                result = IdentityResult.Failed(new IdentityError() { Code = HttpStatusCode.BadRequest.ToString(), Description = $"Argument cannot be null." });
            }
            else
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(identityModel.PartitionKey) ? PartitionKey.None : new PartitionKey(identityModel.PartitionKey);

                    var response = await container.ReplaceItemAsync(identityModel, identityModel.Id, partitionKey, cancellationToken: cancellationToken);

                    result = response.StatusCode >= HttpStatusCode.BadRequest ?
                        IdentityResult.Failed(new IdentityError() { Code = response.StatusCode.ToString(), Description = $"The identity of type {identityModel.GetType().Name} was not updated." }) :
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

        #region Delete Identity Model

        /// <summary>
        ///     Deletes the specified <paramref name="identityModel"/> from the store.
        /// </summary>
        /// <param name="identityModel">The identity model to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.
        /// </returns>
        public async Task<IdentityResult> DeleteAsync<TIdentity>(TIdentity identityModel, CancellationToken cancellationToken = default)
            where TIdentity : ICosmosIdentity, new()
        {
            IdentityResult result;

            cancellationToken.ThrowIfCancellationRequested();

            if (identityModel == null)
            {
                result = IdentityResult.Failed(new IdentityError() { Code = HttpStatusCode.BadRequest.ToString(), Description = $"Argument cannot be null." });
            }
            else
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(identityModel.PartitionKey) ? PartitionKey.None : new PartitionKey(identityModel.PartitionKey);

                    var response = await container.DeleteItemAsync<TIdentity>(identityModel.Id, partitionKey, cancellationToken: cancellationToken);

                    result = response.StatusCode >= HttpStatusCode.BadRequest ?
                        IdentityResult.Failed(new IdentityError() { Code = response.StatusCode.ToString(), Description = $"The identity of type {identityModel.GetType().Name} was not deleted." }) :
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

        #region Find Identity Model

        /// <summary>
        ///     Finds and returns an identity model, if any, who has the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the identity model to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the identity model with the specified <paramref name="id"/> if it exists.
        /// </returns>
        public async Task<TIdentity> FindByIdAsync<TIdentity>(string id, CancellationToken cancellationToken = default)
            where TIdentity : ICosmosIdentity, new()
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var identityModel = new TIdentity();
                    var partitionKey = string.IsNullOrEmpty(identityModel.PartitionKey) ? PartitionKey.None : new PartitionKey(identityModel.PartitionKey);

                    return await container.ReadItemAsync<TIdentity>(id, partitionKey, cancellationToken: cancellationToken);
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
