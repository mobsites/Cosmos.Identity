// Copyright (c) 2020 Allan Mobley. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Identity;
using System.Linq;

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
        ///     A navigation property for the roles the store contains.
        /// </summary>
        public override IQueryable<TRole> Roles => storageProvider.Queryable<TRole>();
    }
}