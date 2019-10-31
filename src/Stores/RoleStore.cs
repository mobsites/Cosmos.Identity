// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     Represents a new instance of a persistence store for the specified types.
    /// </summary>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
    /// <typeparam name="TRoleClaim">The type representing a role claim.</typeparam>
    public class RoleStore<TRole, TUserRole, TRoleClaim> :
        RoleStoreBase<TRole, string, TUserRole, TRoleClaim>
        where TRole : IdentityRole, new()
        where TUserRole : IdentityUserRole, new()
        where TRoleClaim : IdentityRoleClaim, new()
    {
        #region Setup

        private readonly IRoles<TRole> roles;
        private readonly IRoleClaims<TRoleClaim> roleClaims;

        /// <summary>
        ///     Constructs a new instance of <see cref="RoleStore{TRole, TUserRole, TRoleClaim}"/>.
        /// </summary>
        /// <param name="roles">The context used to access the role store.</param>
        /// <param name="roleClaims">The context used to access the role claim store.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public RoleStore(
            IRoles<TRole> roles,
            IRoleClaims<TRoleClaim> roleClaims,
            IdentityErrorDescriber describer = null) : base(describer ?? new IdentityErrorDescriber())
        {
            this.roles = roles ?? throw new ArgumentNullException(nameof(roles));
            this.roleClaims = roleClaims ?? throw new ArgumentNullException(nameof(roleClaims));
        }

        #endregion

        #region Role Store

        #region Roles

        public override IQueryable<TRole> Roles => roles.Queryable;

        #endregion

        #region Create Role

        /// <summary>
        ///     Creates the specified <paramref name="role"/> in the store.
        /// </summary>
        /// <param name="role">The role to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.
        /// </returns>
        public override Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return roles.CreateAsync(role, cancellationToken);
        }

        #endregion

        #region Update Role

        /// <summary>
        ///     Updates the specified <paramref name="role"/> in the store.
        /// </summary>
        /// <param name="role">The role to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.
        /// </returns>
        public override Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            role.ConcurrencyStamp = Guid.NewGuid().ToString();

            return roles.UpdateAsync(role, cancellationToken);
        }

        #endregion

        #region Delete Role

        /// <summary>
        ///     Deletes the specified <paramref name="role"/> from the store.
        /// </summary>
        /// <param name="role">The role to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.
        /// </returns>
        public override Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return roles.DeleteAsync(role, cancellationToken);
        }

        #endregion

        #region Find Role

        /// <summary>
        ///     Finds and returns a role, if any, which has the specified <paramref name="roleId"/>.
        /// </summary>
        /// <param name="roleId">The role ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the role matching the specified <paramref name="roleId"/> if it exists.
        /// </returns>
        public override Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return roles.FindByIdAsync(roleId, cancellationToken);
        }


        /// <summary>
        ///     Finds and returns a role, if any, who has the specified <paramref name="normalizedName"/>.
        /// </summary>
        /// <param name="normalizedName">The role to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the role matching the specified <paramref name="normalizedName"/> if it exists.
        /// </returns>
        public override Task<TRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return roles.FindByNameAsync(normalizedName, cancellationToken);
        }

        #endregion

        #endregion

        #region RoleClaim Store

        #region Add RoleClaim

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

            return roleClaims.AddAsync(CreateRoleClaim(role, claim), cancellationToken);
        }

        #endregion

        #region Remove RoleClaim

        /// <summary>
        ///     Removes the <paramref name="claim"/> given from the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role to remove the claim from.</param>
        /// <param name="claim">The claim to remove from the role.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async override Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
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

            var roleClaimMatches = await roleClaims.FindAsync(role.Id, claim, cancellationToken);
            if (roleClaimMatches != null)
            {
                foreach (var roleClaim in roleClaimMatches)
                {
                    await roleClaims.RemoveAsync(roleClaim, cancellationToken);
                }
            }
        }

        #endregion

        #region Get RoleClaims

        /// <summary>
        ///     Gets the claims associated with the specified <paramref name="role"/> as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose claims should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the claims granted to a role.</returns>
        public override Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return roleClaims.GetClaimsAsync(role.Id, cancellationToken);
        }

        #endregion

        #endregion
    }
}
