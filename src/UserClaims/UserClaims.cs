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
    ///     Represents a new instance of a persistence store for the identity user claims.
    /// </summary>
    /// <typeparam name="TUserClaim">The type representing a user claim.</typeparam>
    public class UserClaims<TUserClaim> : IUserClaims<TUserClaim>
        where TUserClaim : IdentityUserClaim, new()
    {
        #region Setup

        private readonly ICosmosIdentityContainer cosmos;

        /// <summary>
        ///     Constructs a new instance of <see cref="UserClaims{TUserClaim}"/>.
        /// </summary>
        /// <param name="cosmos">The context in which to access the Cosmos Container for the identity store.</param>
        public UserClaims(ICosmosIdentityContainer cosmos)
        {
            this.cosmos = cosmos ?? throw new ArgumentNullException(nameof(cosmos));
        }

        #endregion

        #region Add UserClaim

        /// <summary>
        ///     Adds the given <paramref name="userClaim"/> to the store.
        /// </summary>
        /// <param name="userClaim">The user claim to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async Task AddAsync(TUserClaim userClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (userClaim != null)
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(userClaim.PartitionKey) ? PartitionKey.None : new PartitionKey(userClaim.PartitionKey);

                    await cosmos.IdentityContainer.CreateItemAsync(userClaim, partitionKey, cancellationToken: cancellationToken);
                }
                catch (CosmosException)
                {

                }
            }
        }

        #endregion

        #region Update UserClaim

        /// <summary>
        ///     Updates the given <paramref name="userClaim"/> in the store.
        /// </summary>
        /// <param name="userClaim">The user claim to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async Task UpdateAsync(TUserClaim userClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (userClaim != null)
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(userClaim.PartitionKey) ? PartitionKey.None : new PartitionKey(userClaim.PartitionKey);

                    await cosmos.IdentityContainer.ReplaceItemAsync(userClaim, userClaim.Id, partitionKey, cancellationToken: cancellationToken);
                }
                catch (CosmosException)
                {

                }
            }
        }

        #endregion

        #region Remove UserClaim

        /// <summary>
        ///     Removes the given <paramref name="userClaim"/> from the store.
        /// </summary>
        /// <param name="userClaim">The user claim to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async Task RemoveAsync(TUserClaim userClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (userClaim != null)
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(userClaim.PartitionKey) ? PartitionKey.None : new PartitionKey(userClaim.PartitionKey);

                    await cosmos.IdentityContainer.DeleteItemAsync<TUserClaim>(userClaim.Id, partitionKey, cancellationToken: cancellationToken);
                }
                catch (CosmosException)
                {

                }
            }
        }

        #endregion

        #region Get Claims

        /// <summary>
        ///     Retrieves the claims for the user with the given <paramref name="userId"/> from the store.
        /// </summary>
        /// <param name="userId">The id of the user to get claims for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The claims for the user if any.
        /// </returns>
        public async Task<IList<Claim>> GetClaimsAsync(string userId, CancellationToken cancellationToken)
        {
            IList<Claim> claims = new List<Claim>();

            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    var partitionKey = new TUserClaim().PartitionKey;

                    // LINQ query generation
                    var feedIterator = cosmos.IdentityContainer
                        .GetItemLinqQueryable<TUserClaim>(requestOptions: new QueryRequestOptions
                        {
                            PartitionKey = string.IsNullOrEmpty(partitionKey) ? PartitionKey.None : new PartitionKey(partitionKey)
                        })
                        .Where(userClaim => userClaim.UserId == userId)
                        .ToFeedIterator();

                    //Asynchronous query execution
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var userClaim in await feedIterator.ReadNextAsync())
                        {
                            claims.Add(userClaim.ToClaim());
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

        #region Get Claim Users

        /// <summary>
        ///     Retrieves a list of users from the store that have the specified <paramref name="claim"/>.
        /// </summary>
        /// <param name="claim">The claim to match to users.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The list of users if any.</returns>
        public async Task<IList<TUser>> GetUsersAsync<TUser>(Claim claim, CancellationToken cancellationToken)
            where TUser : IdentityUser, new()
        {
            IList<TUser> users = new List<TUser>();

            cancellationToken.ThrowIfCancellationRequested();

            if (claim != null)
            {
                try
                {
                    var partitionKey = new TUser().PartitionKey;

                    // LINQ query generation
                    var feedIterator = cosmos.IdentityContainer
                        .GetItemLinqQueryable<TUser>(requestOptions: new QueryRequestOptions
                        {
                            PartitionKey = string.IsNullOrEmpty(partitionKey) ? PartitionKey.None : new PartitionKey(partitionKey)
                        })
                        .Where(user => !string.IsNullOrEmpty(user.FlattenClaims) && user.FlattenClaims.Contains($"{claim.Type}|{claim.Value}"))
                        .ToFeedIterator();

                    //Asynchronous query execution
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var user in await feedIterator.ReadNextAsync())
                        {
                            users.Add(user);
                        }
                    }
                }
                catch (CosmosException)
                {

                }
            }

            return users;
        }

        #endregion

        #region Find UserClaims

        /// <summary>
        ///     Retrieves the user claims matching the given <paramref name="claim"/> for the user with the given <paramref name="userId"/> from the store.
        /// </summary>
        /// <param name="userId">The id of the user to get claims for.</param>
        /// <param name="claim">The claim to match.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The matching user claims if any.
        /// </returns>
        public async Task<IList<TUserClaim>> FindAsync(string userId, Claim claim, CancellationToken cancellationToken)
        {
            IList<TUserClaim> userClaims = new List<TUserClaim>();

            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    var partitionKey = new TUserClaim().PartitionKey;

                    // LINQ query generation
                    var feedIterator = cosmos.IdentityContainer
                        .GetItemLinqQueryable<TUserClaim>(requestOptions: new QueryRequestOptions
                        {
                            PartitionKey = string.IsNullOrEmpty(partitionKey) ? PartitionKey.None : new PartitionKey(partitionKey)
                        })
                        .Where(userClaim => userClaim.UserId == userId && userClaim.ClaimType == claim.Type && userClaim.ClaimValue == claim.Value)
                        .ToFeedIterator();

                    //Asynchronous query execution
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var userClaim in await feedIterator.ReadNextAsync())
                        {
                            userClaims.Add(userClaim);
                        }
                    }
                }
                catch (CosmosException)
                {

                }
            }

            return userClaims;
        }

        #endregion
    }
}
