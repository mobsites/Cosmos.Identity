// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     Represents a new instance of a persistence store for the identity user tokens.
    /// </summary>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    public class UserTokens<TUserToken> : IUserTokens<TUserToken>
        where TUserToken : IdentityUserToken, new()
    {
        #region Setup

        private readonly ICosmos cosmos;

        /// <summary>
        ///     Constructs a new instance of <see cref="UserTokens{TUserToken}"/>.
        /// </summary>
        /// <param name="cosmos">The context in which to access the Cosmos Container for the identity store.</param>
        public UserTokens(ICosmos cosmos)
        {
            this.cosmos = cosmos ?? throw new ArgumentNullException(nameof(cosmos));
        }

        #endregion

        #region Add UserToken

        /// <summary>
        ///     Adds a new user token to the store.
        /// </summary>
        /// <param name="token">The token to add.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async Task AddAsync(TUserToken token)
        {
            if (token != null)
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(token.PartitionKey) ? PartitionKey.None : new PartitionKey(token.PartitionKey);

                    await cosmos.IdentityContainer.CreateItemAsync(token, partitionKey);
                }
                catch (CosmosException)
                {
                    
                }
            }
        }

        #endregion

        #region Remove UserToken

        /// <summary>
        ///     Removes a user token from the store.
        /// </summary>
        /// <param name="token">The token to remove.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async Task RemoveAsync(TUserToken token)
        {
            if (token != null)
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(token.PartitionKey) ? PartitionKey.None : new PartitionKey(token.PartitionKey);

                    await cosmos.IdentityContainer.DeleteItemAsync<TUserToken>(token.Id, partitionKey);
                }
                catch (CosmosException)
                {

                }
            }
        }

        #endregion

        #region Get UserTokens

        /// <summary>
        ///     Retrieves user tokens from store if any.
        /// </summary>
        /// <param name="userId">The token owner's id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user token if it exists.</returns>
        public async Task<IList<TUserToken>> GetTokensAsync(string userId, CancellationToken cancellationToken)
        {
            IList<TUserToken> userTokens = new List<TUserToken>();

            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    userTokens = cosmos.IdentityContainer.GetItemLinqQueryable<TUserToken>(allowSynchronousQueryExecution: true)
                        .Where(u => u.UserId == userId)
                        .ToList();

                    var userToken = new TUserToken();
                    var partitionKey = string.IsNullOrEmpty(userToken.PartitionKey) ? PartitionKey.None : new PartitionKey(userToken.PartitionKey);

                    // LINQ query generation
                    var feedIterator = cosmos.IdentityContainer.GetItemLinqQueryable<TUserToken>(requestOptions: new QueryRequestOptions
                        {
                            PartitionKey = partitionKey
                        })
                        .Where(u => u.UserId == userId)
                        .ToFeedIterator();

                    //Asynchronous query execution
                    while (feedIterator.HasMoreResults)
                    {
                        // Should only be one.
                        foreach (var token in await feedIterator.ReadNextAsync())
                        {
                            userTokens.Add(token);
                        }
                    }
                }
                catch (CosmosException)
                {

                }
            }

            return userTokens;
        }

        #endregion

        #region Find UserToken

        /// <summary>
        ///     Retrieves a user token from store if it exists.
        /// </summary>
        /// <param name="userId">The token owner's id.</param>
        /// <param name="loginProvider">The login provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user token if it exists.</returns>
        public async Task<TUserToken> FindAsync(string userId, string loginProvider, string name, CancellationToken cancellationToken)
        {
            TUserToken userToken = new TUserToken();

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var partitionKey = string.IsNullOrEmpty(userToken.PartitionKey) ? PartitionKey.None : new PartitionKey(userToken.PartitionKey);

                // LINQ query generation
                var feedIterator = cosmos.IdentityContainer.GetItemLinqQueryable<TUserToken>(requestOptions: new QueryRequestOptions
                    {
                        PartitionKey = partitionKey
                    })
                    .Where(u => u.UserId == userId && u.LoginProvider == loginProvider && u.Name == name)
                    .ToFeedIterator();

                //Asynchronous query execution
                while (feedIterator.HasMoreResults)
                {
                    // Should only be one.
                    foreach (var token in await feedIterator.ReadNextAsync())
                    {
                        userToken = token;
                    }
                }
            }
            catch (CosmosException)
            {

            }

            return userToken;
        }

        #endregion
    }
}
