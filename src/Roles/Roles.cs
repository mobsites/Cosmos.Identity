// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     Represents a new instance of a persistence store for the identity roles.
    /// </summary>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    public class Roles<TRole> : IRoles<TRole>
        where TRole : IdentityRole, new()
    {
        #region Setup

        private readonly ICosmosIdentityContainer cosmos;

        /// <summary>
        ///     Constructs a new instance of <see cref="Roles{TRole}"/>.
        /// </summary>
        /// <param name="cosmos">The context in which to access the Cosmos Container for the identity store.</param>
        public Roles(ICosmosIdentityContainer cosmos)
        {
            this.cosmos = cosmos ?? throw new ArgumentNullException(nameof(cosmos));
        }

        #endregion

        /// <summary>
        ///     A navigation property for the roles the store contains.
        /// </summary>
        public IQueryable<TRole> Queryable => cosmos.IdentityContainer.GetItemLinqQueryable<TRole>(allowSynchronousQueryExecution: true);

        #region Create Role

        /// <summary>
        ///     Creates the specified <paramref name="role"/> in the store.
        /// </summary>
        /// <param name="role">The role to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.
        /// </returns>
        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            IdentityResult result;

            cancellationToken.ThrowIfCancellationRequested();

            if (role is null)
            {
                result = IdentityResult.Failed(new IdentityError() { Code = HttpStatusCode.BadRequest.ToString(), Description = $"Argument cannot be null." });
            }
            else
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(role.PartitionKey) ? PartitionKey.None : new PartitionKey(role.PartitionKey);

                    var response = await cosmos.IdentityContainer.CreateItemAsync(role, partitionKey, cancellationToken: cancellationToken);

                    result = response.StatusCode >= HttpStatusCode.BadRequest ?
                        IdentityResult.Failed(new IdentityError() { Code = response.StatusCode.ToString(), Description = $"The role {role.Name} was not created." }) :
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

        #region Update Role

        /// <summary>
        ///     Updates the specified <paramref name="role"/> in the store.
        /// </summary>
        /// <param name="role">The role to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.
        /// </returns>
        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            IdentityResult result;

            cancellationToken.ThrowIfCancellationRequested();

            if (role is null)
            {
                result = IdentityResult.Failed(new IdentityError() { Code = HttpStatusCode.BadRequest.ToString(), Description = $"Argument cannot be null." });
            }
            else
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(role.PartitionKey) ? PartitionKey.None : new PartitionKey(role.PartitionKey);

                    var response = await cosmos.IdentityContainer.ReplaceItemAsync(role, role.Id, partitionKey, cancellationToken: cancellationToken);

                    result = response.StatusCode >= HttpStatusCode.BadRequest ?
                        IdentityResult.Failed(new IdentityError() { Code = response.StatusCode.ToString(), Description = $"The role {role.Name} was not updated." }) :
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

        #region Delete Role

        /// <summary>
        ///     Deletes the specified <paramref name="role"/> from the store.
        /// </summary>
        /// <param name="role">The role to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.
        /// </returns>
        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            IdentityResult result;

            cancellationToken.ThrowIfCancellationRequested();

            if (role is null)
            {
                result = IdentityResult.Failed(new IdentityError() { Code = HttpStatusCode.BadRequest.ToString(), Description = $"Argument cannot be null." });
            }
            else
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(role.PartitionKey) ? PartitionKey.None : new PartitionKey(role.PartitionKey);

                    var response = await cosmos.IdentityContainer.DeleteItemAsync<TRole>(role.Id, partitionKey, cancellationToken: cancellationToken);

                    result = response.StatusCode >= HttpStatusCode.BadRequest ?
                        IdentityResult.Failed(new IdentityError() { Code = response.StatusCode.ToString(), Description = $"The role {role.Name} was not deleted." }) :
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

        #region Find Role

        /// <summary>
        ///     Finds and returns a role, if any, who has the specified <paramref name="roleId"/>.
        /// </summary>
        /// <param name="roleId">The role ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the role matching the specified <paramref name="roleId"/> if it exists.
        /// </returns>
        public async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(roleId))
            {
                try
                {
                    var role = new TRole();
                    var partitionKey = string.IsNullOrEmpty(role.PartitionKey) ? PartitionKey.None : new PartitionKey(role.PartitionKey);

                    return await cosmos.IdentityContainer.ReadItemAsync<TRole>(roleId, partitionKey, cancellationToken: cancellationToken);
                }
                catch (CosmosException)
                {

                }
            }

            return null;
        }

        /// <summary>
        ///     Finds and returns a role, if any, who has the specified <paramref name="normalizedName"/>.
        /// </summary>
        /// <param name="normalizedName">The role to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the role matching the specified <paramref name="normalizedName"/> if it exists.
        /// </returns>
        public async Task<TRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(normalizedName))
            {
                try
                {
                    var partitionKey = new TRole().PartitionKey;

                    // LINQ query generation
                    var feedIterator = cosmos.IdentityContainer
                        .GetItemLinqQueryable<TRole>(requestOptions: new QueryRequestOptions
                        {
                            PartitionKey = string.IsNullOrEmpty(partitionKey) ? PartitionKey.None : new PartitionKey(partitionKey)
                        })
                        .Where(role => role.NormalizedName == normalizedName)
                        .ToFeedIterator();

                    //Asynchronous query execution
                    while (feedIterator.HasMoreResults)
                    {
                        // Should only be one, so...
                        return (await feedIterator.ReadNextAsync()).First();
                    }
                }
                catch (CosmosException)
                {

                }
            }

            return null;
        }

        #endregion
    }
}
