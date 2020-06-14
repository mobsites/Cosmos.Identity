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
    public partial class RoleStore<TCustomStorageProvider, TRole, TUserRole, TRoleClaim> :
        RoleStoreBase<TRole, string, TUserRole, TRoleClaim>
        where TCustomStorageProvider : IIdentityStorageProvider
        where TRole : IdentityRole, new()
        where TUserRole : IdentityUserRole, new()
        where TRoleClaim : IdentityRoleClaim, new()
    {
        /// <summary>
        ///     Retrieves the role claims matching the given <paramref name="claim"/> for the role with the given <paramref name="roleId"/> from the store.
        /// </summary>
        /// <param name="roleId">The id of the role to get claims for.</param>
        /// <param name="claim">The claim to match.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The matching role claims if any.
        /// </returns>
        protected async Task<IList<TRoleClaim>> FindClaimsAsync(string roleId, Claim claim, CancellationToken cancellationToken)
        {
            IList<TRoleClaim> roleClaims = new List<TRoleClaim>();

            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (!string.IsNullOrEmpty(roleId))
            {
                try
                {
                    // LINQ query generation
                    var feedIterator = storageProvider.Queryable<TRoleClaim>()
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
    }
}