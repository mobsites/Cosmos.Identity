// Copyright (c) 2020 Allan Mobley. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.Cosmos.Identity
{
    public partial class UserStore<TCustomStorageProvider, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> :
        UserStoreBase<TUser, TRole, string, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>,
        IProtectedUserStore<TUser>
        where TCustomStorageProvider : IIdentityStorageProvider
        where TUser : IdentityUser, new()
        where TRole : IdentityRole, new()
        where TUserClaim : IdentityUserClaim, new()
        where TUserRole : IdentityUserRole, new()
        where TUserLogin : IdentityUserLogin, new()
        where TUserToken : IdentityUserToken, new()
        where TRoleClaim : IdentityRoleClaim, new()
    {
        /// <summary>
        ///     Returns a user login with  provider, providerKey if it exists.
        /// </summary>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        protected async override Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(loginProvider))
            {
                throw new ArgumentException($"{nameof(loginProvider)} cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(providerKey))
            {
                throw new ArgumentException($"{nameof(providerKey)} cannot be null or empty.");
            }

            try
            {
                // LINQ query generation
                var feedIterator = storageProvider.Queryable<TUserLogin>()
                    .Where(userLogin => userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey)
                    .ToFeedIterator();

                //Asynchronous query execution
                while (feedIterator.HasMoreResults)
                {
                    // Should only be one, so...
                    return (await feedIterator.ReadNextAsync()).FirstOrDefault();
                }
            }
            catch (CosmosException)
            {

            }

            return null;
        }

        /// <summary>
        ///     Returns a user login with the matching userId, provider, providerKey if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        protected async override Task<TUserLogin> FindUserLoginAsync(string userId, string loginProvider, string providerKey, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException($"{nameof(userId)} cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(loginProvider))
            {
                throw new ArgumentException($"{nameof(loginProvider)} cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(providerKey))
            {
                throw new ArgumentException($"{nameof(providerKey)} cannot be null or empty.");
            }

            try
            {
                // LINQ query generation
                var feedIterator = storageProvider.Queryable<TUserLogin>()
                    .Where(userLogin => userLogin.UserId == userId && userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey)
                    .ToFeedIterator();

                //Asynchronous query execution
                while (feedIterator.HasMoreResults)
                {
                    // Should only be one, so...
                    return (await feedIterator.ReadNextAsync()).FirstOrDefault();
                }
            }
            catch (CosmosException)
            {

            }

            return null;
        }
    }
}