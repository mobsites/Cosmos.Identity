// Copyright (c) 2020 Allan Mobley. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Identity;
using System;

namespace Mobsites.Cosmos.Identity
{
    /// <summary>
    ///     Represents a new instance of a persistence store for the specified types.
    /// </summary>
    /// <typeparam name="TCustomStorageProvider">The type representing a Cosmos storage provider.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
    /// <typeparam name="TRoleClaim">The type representing a role claim.</typeparam>
    public partial class RoleStore<TCustomStorageProvider, TRole, TUserRole, TRoleClaim> :
        RoleStoreBase<TRole, string, TUserRole, TRoleClaim>
        where TCustomStorageProvider : IIdentityStorageProvider
        where TRole : IdentityRole, new()
        where TUserRole : IdentityUserRole, new()
        where TRoleClaim : IdentityRoleClaim, new()
    {
        private readonly TCustomStorageProvider storageProvider;

        /// <summary>
        ///     Constructs a new instance of <see cref="RoleStore{TCustomStorageProvider, TRole, TUserRole, TRoleClaim}"/>.
        /// </summary>
        /// <param name="storageProvider">The provider used to access the store.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public RoleStore(TCustomStorageProvider storageProvider, IdentityErrorDescriber describer = null)
            : base(describer ?? new IdentityErrorDescriber())
        {
            this.storageProvider = storageProvider ?? throw new ArgumentNullException(nameof(storageProvider));
        }
    }
}