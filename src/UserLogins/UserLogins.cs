// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Identity;
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
    ///     Represents a new instance of a persistence store for the identity user logins.
    /// </summary>
    /// <typeparam name="TUserLogin">The type representing a user login.</typeparam>
    public class UserLogins<TUserLogin> : IUserLogins<TUserLogin>
        where TUserLogin : IdentityUserLogin, new()
    {
        #region Setup

        private readonly ICosmos cosmos;

        /// <summary>
        ///     Constructs a new instance of <see cref="UserLogins{TUserLogin}"/>.
        /// </summary>
        /// <param name="cosmos">The context in which to access the Cosmos Container for the identity store.</param>
        public UserLogins(ICosmos cosmos)
        {
            this.cosmos = cosmos ?? throw new ArgumentNullException(nameof(cosmos));
        }

        #endregion

        #region Add UserLogin

        /// <summary>
        ///     Adds the given <paramref name="userLogin"/> to the store.
        /// </summary>
        /// <param name="userLogin">The user login to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async Task AddAsync(TUserLogin userLogin, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (userLogin != null)
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(userLogin.PartitionKey) ? PartitionKey.None : new PartitionKey(userLogin.PartitionKey);

                    await cosmos.IdentityContainer.CreateItemAsync(userLogin, partitionKey, cancellationToken: cancellationToken);
                }
                catch (CosmosException)
                {

                }
            }
        }

        #endregion

        #region Remove UserLogin

        /// <summary>
        ///     Removes the given <paramref name="userLogin"/> from the store.
        /// </summary>
        /// <param name="userLogin">The user login to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async Task RemoveAsync(TUserLogin userLogin, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (userLogin != null)
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(userLogin.PartitionKey) ? PartitionKey.None : new PartitionKey(userLogin.PartitionKey);

                    await cosmos.IdentityContainer.DeleteItemAsync<TUserLogin>(userLogin.Id, partitionKey, cancellationToken: cancellationToken);
                }
                catch (CosmosException)
                {

                }
            }
        }

        #endregion

        #region Get UserLoginInfo

        /// <summary>
        ///     Retrieves the associated logins for the user with the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The id of the user whose associated logins to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing a list of <see cref="UserLoginInfo"/> for the specified <paramref name="user"/>, if any.
        /// </returns>
        public async Task<IList<UserLoginInfo>> GetLoginsAsync(string userId, CancellationToken cancellationToken)
        {
            IList<UserLoginInfo> userLognins = new List<UserLoginInfo>();

            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    var partitionKey = new TUserLogin().PartitionKey;

                    // LINQ query generation
                    var feedIterator = cosmos.IdentityContainer
                        .GetItemLinqQueryable<TUserLogin>(requestOptions: new QueryRequestOptions
                        {
                            PartitionKey = string.IsNullOrEmpty(partitionKey) ? PartitionKey.None : new PartitionKey(partitionKey)
                        })
                        .Where(userLogin => userLogin.UserId == userId)
                        .ToFeedIterator();

                    //Asynchronous query execution
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var userLogin in await feedIterator.ReadNextAsync())
                        {
                            userLognins.Add(new UserLoginInfo(userLogin.LoginProvider, userLogin.ProviderKey, userLogin.ProviderDisplayName));
                        }
                    }
                }
                catch (CosmosException)
                {

                }
            }

            return userLognins;
        }

        #endregion

        #region Find UserLogin

        /// <summary>
        ///     Returns a user login with the matching provider and providerKey if it exists.
        /// </summary>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        public async Task<TUserLogin> FindAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var partitionKey = new TUserLogin().PartitionKey;

                // LINQ query generation
                var feedIterator = cosmos.IdentityContainer
                    .GetItemLinqQueryable<TUserLogin>(requestOptions: new QueryRequestOptions
                    {
                        PartitionKey = string.IsNullOrEmpty(partitionKey) ? PartitionKey.None : new PartitionKey(partitionKey)
                    })
                    .Where(userLogin => userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey)
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

            return null;
        }


        /// <summary>
        ///     Returns a user login with the matching userId, provider, and providerKey if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        public async Task<TUserLogin> FindAsync(string userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var partitionKey = new TUserLogin().PartitionKey;

                // LINQ query generation
                var feedIterator = cosmos.IdentityContainer
                    .GetItemLinqQueryable<TUserLogin>(requestOptions: new QueryRequestOptions
                    {
                        PartitionKey = string.IsNullOrEmpty(partitionKey) ? PartitionKey.None : new PartitionKey(partitionKey)
                    })
                    .Where(userLogin => userLogin.UserId == userId && userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey)
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

            return null;
        }

        #endregion
    }
}
