// Copyright (c) 2020 Allan Mobley. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
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
        ///     Retrieves the names of the roles the specified <paramref name="user"/> is a member of.
        /// </summary>
        /// <param name="user">The user whose roles should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     A <see cref="Task{TResult}"/> that contains the names of the roles the user is a member of.
        /// </returns>
        public override Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default)
        {
            IList<string> roleNames = new List<string>();

            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                if (!string.IsNullOrEmpty(user.FlattenRoleNames))
                {
                    foreach (var roleName in user.FlattenRoleNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        roleNames.Add(roleName);
                    }
                }
            }
            catch (Exception)
            {

            }

            return Task.FromResult(roleNames);
        }
    }
}