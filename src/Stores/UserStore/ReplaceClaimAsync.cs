// Copyright (c) 2020 Allan Mobley. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Identity;
using System;
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
        ///     Replaces the <paramref name="claim"/> on the specified <paramref name="user"/>, with the <paramref name="newClaim"/>.
        /// </summary>
        /// <param name="user">The user to replace the claim on.</param>
        /// <param name="claim">The claim to replace.</param>
        /// <param name="newClaim">The new claim replacing the <paramref name="claim"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async override Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (claim is null)
            {
                throw new ArgumentNullException(nameof(claim));
            }
            if (newClaim is null)
            {
                throw new ArgumentNullException(nameof(newClaim));
            }

            foreach (var userClaim in await FindClaimsAsync(user.Id, claim, cancellationToken))
            {
                var oldClaim = userClaim;

                userClaim.ClaimType = newClaim.Type;
                userClaim.ClaimValue = newClaim.Value;

                var result = await storageProvider.UpdateAsync(userClaim, cancellationToken);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(user.FlattenClaims))
                    {
                        user.FlattenClaims.Replace($"{oldClaim.ClaimType}|{oldClaim.ClaimValue}", $"{newClaim.Type}|{newClaim.Value}");
                    }
                }
            }
        }
    }
}