// Copyright (c) 2020 Allan Mobley. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Collections.Generic;
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
        ///     Retrieves user tokens from store if any.
        /// </summary>
        /// <param name="userId">The token owner's id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user token if it exists.</returns>
        public async Task<IList<TUserToken>> GetTokensAsync(string userId, CancellationToken cancellationToken)
        {
            IList<TUserToken> userTokens = new List<TUserToken>();

            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    // LINQ query generation
                    var feedIterator = storageProvider.Queryable<TUserToken>()
                        .Where(userToken => userToken.UserId == userId)
                        .ToFeedIterator();

                    //Asynchronous query execution
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var userToken in await feedIterator.ReadNextAsync())
                        {
                            userTokens.Add(userToken);
                        }
                    }
                }
                catch (CosmosException)
                {

                }
            }

            return userTokens;
        }
    }
}