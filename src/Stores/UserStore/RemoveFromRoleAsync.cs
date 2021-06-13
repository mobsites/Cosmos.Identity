// Copyright (c) 2020 Allan Mobley. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Identity;
using System;
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
        ///     Removes the given <paramref name="normalizedRoleName"/> from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove from the role.</param>
        /// <param name="normalizedRoleName">The role to remove from.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async override Task RemoveFromRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrEmpty(normalizedRoleName))
            {
                throw new ArgumentException($"{nameof(normalizedRoleName)} cannot be null or empty.");
            }

            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (role != null)
            {
                var userRole = await FindUserRoleAsync(user.Id, role.Id, cancellationToken);
                if (userRole != null)
                {
                    var result = await storageProvider.DeleteAsync(userRole, cancellationToken);

                    if (result.Succeeded)
                    {
                        // Update user object (default UserManager will actually call UpdateUserAsync(user).
                        if (!string.IsNullOrEmpty(user.FlattenRoleNames))
                        {
                            user.FlattenRoleNames = user.FlattenRoleNames.Replace(role.Name + ",", string.Empty);
                        }
                        if (!string.IsNullOrEmpty(user.FlattenRoleIds))
                        {
                            user.FlattenRoleIds = user.FlattenRoleIds.Replace(role.Id + ",", string.Empty);
                        }
                    }
                }
            }
        }
    }
}