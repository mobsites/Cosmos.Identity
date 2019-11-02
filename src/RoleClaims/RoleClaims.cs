// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     Represents a new instance of a persistence store for the identity role claims.
    /// </summary>
    /// <typeparam name="TRoleClaim">The type representing a role claim.</typeparam>
    public class RoleClaims<TRoleClaim> : IRoleClaims<TRoleClaim>
        where TRoleClaim : IdentityRoleClaim, new()
    {
        #region Setup

        private readonly ICosmosIdentityContainer cosmos;

        /// <summary>
        ///     Constructs a new instance of <see cref="RoleClaims{TRoleClaim}"/>.
        /// </summary>
        /// <param name="cosmos">The context in which to access the Cosmos Container for the identity store.</param>
        public RoleClaims(ICosmosIdentityContainer cosmos)
        {
            this.cosmos = cosmos ?? throw new ArgumentNullException(nameof(cosmos));
        }

        #endregion

        #region Add RoleClaim

        /// <summary>
        ///     Adds the given <paramref name="roleClaim"/> to the store.
        /// </summary>
        /// <param name="roleClaim">The role claim to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async Task AddAsync(TRoleClaim roleClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (roleClaim != null)
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(roleClaim.PartitionKey) ? PartitionKey.None : new PartitionKey(roleClaim.PartitionKey);

                    await cosmos.IdentityContainer.CreateItemAsync(roleClaim, partitionKey, cancellationToken: cancellationToken);
                }
                catch (CosmosException)
                {

                }
            }
        }

        #endregion

        #region Remove RoleClaim

        /// <summary>
        ///     Removes the given <paramref name="roleClaim"/> from the store.
        /// </summary>
        /// <param name="roleClaim">The role claim to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async Task RemoveAsync(TRoleClaim roleClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (roleClaim != null)
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(roleClaim.PartitionKey) ? PartitionKey.None : new PartitionKey(roleClaim.PartitionKey);

                    await cosmos.IdentityContainer.DeleteItemAsync<TRoleClaim>(roleClaim.Id, partitionKey, cancellationToken: cancellationToken);
                }
                catch (CosmosException)
                {

                }
            }
        }

        #endregion

        #region Get Claims

        /// <summary>
        ///     Retrieves the claims for the role with the given <paramref name="roleId"/> from the store.
        /// </summary>
        /// <param name="roleId">The id of the role to get claims for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The claims for the role if any.
        /// </returns>
        public async Task<IList<Claim>> GetClaimsAsync(string roleId, CancellationToken cancellationToken)
        {
            IList<Claim> claims = new List<Claim>();

            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(roleId))
            {
                try
                {
                    var partitionKey = new TRoleClaim().PartitionKey;

                    // LINQ query generation
                    var feedIterator = cosmos.IdentityContainer
                        .GetItemLinqQueryable<TRoleClaim>(requestOptions: new QueryRequestOptions
                        {
                            PartitionKey = string.IsNullOrEmpty(partitionKey) ? PartitionKey.None : new PartitionKey(partitionKey)
                        })
                        .Where(roleClaim => roleClaim.RoleId == roleId)
                        .ToFeedIterator();

                    //Asynchronous query execution
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var roleClaim in await feedIterator.ReadNextAsync())
                        {
                            claims.Add(roleClaim.ToClaim());
                        }
                    }
                }
                catch (CosmosException)
                {

                }
            }

            return claims;
        }

        #endregion

        #region Find RoleClaims

        /// <summary>
        ///     Retrieves the role claims matching the given <paramref name="claim"/> for the role with the given <paramref name="roleId"/> from the store.
        /// </summary>
        /// <param name="roleId">The id of the role to get claims for.</param>
        /// <param name="claim">The claim to match.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The matching role claims if any.
        /// </returns>
        public async Task<IList<TRoleClaim>> FindAsync(string roleId, Claim claim, CancellationToken cancellationToken)
        {
            IList<TRoleClaim> roleClaims = new List<TRoleClaim>();

            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(roleId))
            {
                try
                {
                    var partitionKey = new TRoleClaim().PartitionKey;

                    // LINQ query generation
                    var feedIterator = cosmos.IdentityContainer
                        .GetItemLinqQueryable<TRoleClaim>(requestOptions: new QueryRequestOptions
                        {
                            PartitionKey = string.IsNullOrEmpty(partitionKey) ? PartitionKey.None : new PartitionKey(partitionKey)
                        })
                        .Where(roleClaim => roleClaim.RoleId == roleId && roleClaim.ClaimType == claim.Type && roleClaim.ClaimValue == claim.Value)
                        .ToFeedIterator();

                    //Asynchronous query execution
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var roleClaim in await feedIterator.ReadNextAsync())
                        {
                            roleClaims.Add(roleClaim);
                        }
                    }
                }
                catch (CosmosException)
                {

                }
            }

            return roleClaims;
        }

        #endregion
    }
}
