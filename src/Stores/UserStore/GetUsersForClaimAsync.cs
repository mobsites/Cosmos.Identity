// Copyright (c) 2020 Allan Mobley. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
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
        ///     Retrieves the users with the given <paramref name="claim"/> from the store.
        /// </summary>
        /// <param name="claim">The claim to get users for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The users with the claim if any.
        /// </returns>
        public async override Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default)
        {
            IList<TUser> users = new List<TUser>();

            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (claim is null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            try
            {
                // LINQ query generation
                var feedIterator = storageProvider.Queryable<TUser>()
                    .Where(user => user.FlattenClaims.Contains($"{claim.Type}|{claim.Value}"))
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

            return users;
        }
    }
}