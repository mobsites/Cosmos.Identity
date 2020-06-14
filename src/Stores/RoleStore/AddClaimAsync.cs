// Copyright (c) 2020 Allan Mobley. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Identity;
using System;
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
        ///     Adds the <paramref name="claim"/> given to the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role to add the claim to.</param>
        /// <param name="claim">The claim to add to the role.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (claim is null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            return storageProvider.CreateAsync(CreateRoleClaim(role, claim), cancellationToken);
        }
    }
}