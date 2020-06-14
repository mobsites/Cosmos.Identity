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
    public partial class RoleStore<TCustomStorageProvider, TRole, TUserRole, TRoleClaim> :
        RoleStoreBase<TRole, string, TUserRole, TRoleClaim>
        where TCustomStorageProvider : IIdentityStorageProvider
        where TRole : IdentityRole, new()
        where TUserRole : IdentityUserRole, new()
        where TRoleClaim : IdentityRoleClaim, new()
    {
        /// <summary>
        ///     Gets the claims associated with the specified <paramref name="role"/> as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose claims should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the claims granted to a role.</returns>
        public async override Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default)
        {
            IList<Claim> claims = new List<Claim>();

            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            try
            {
                // LINQ query generation
                var feedIterator = storageProvider.Queryable<TRoleClaim>()
                    .Where(roleClaim => roleClaim.RoleId == role.Id)
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

            return claims;
        }
    }
}