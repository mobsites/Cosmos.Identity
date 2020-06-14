// Copyright (c) 2020 Allan Mobley. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        ///     Retrieves the user claims matching the given <paramref name="claim"/> for the user with the given <paramref name="userId"/> from the store.
        /// </summary>
        /// <param name="userId">The id of the user to get claims for.</param>
        /// <param name="claim">The claim to match.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The matching user claims if any.
        /// </returns>
        public async Task<IList<TUserClaim>> FindClaimsAsync(string userId, Claim claim, CancellationToken cancellationToken)
        {
            IList<TUserClaim> userClaims = new List<TUserClaim>();

            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    // LINQ query generation
                    var feedIterator = storageProvider.Queryable<TUserClaim>()
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
    }
}