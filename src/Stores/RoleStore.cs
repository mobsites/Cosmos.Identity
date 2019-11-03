// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
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

        private readonly ICosmosIdentityStorageProvider storageProvider;

        /// <summary>
        ///     Constructs a new instance of <see cref="RoleStore{TRole, TUserRole, TRoleClaim}"/>.
        /// </summary>
        /// <param name="storageProvider">The provider used to access the store.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public RoleStore(ICosmosIdentityStorageProvider storageProvider, IdentityErrorDescriber describer = null)
            : base(describer ?? new IdentityErrorDescriber())
        {
            this.storageProvider = storageProvider ?? throw new ArgumentNullException(nameof(storageProvider));
        }

        #endregion

        #region Role Store

        /// <summary>
        ///     A navigation property for the roles the store contains.
        /// </summary>
        public override IQueryable<TRole> Roles => storageProvider.Queryable<TRole>();

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

            return storageProvider.CreateAsync(role, cancellationToken);
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

            return storageProvider.UpdateAsync(role, cancellationToken);
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
        public async override Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            var result = await storageProvider.DeleteAsync(role, cancellationToken);
            if (result.Succeeded)
            {
                await RemoveClaimsAsync(role, cancellationToken);
            }

            return result;
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

            return storageProvider.FindByIdAsync<TRole>(roleId, cancellationToken);
        }


        /// <summary>
        ///     Finds and returns a role, if any, who has the specified <paramref name="normalizedName"/>.
        /// </summary>
        /// <param name="normalizedName">The role to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the role matching the specified <paramref name="normalizedName"/> if it exists.
        /// </returns>
        public async override Task<TRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (!string.IsNullOrEmpty(normalizedName))
            {
                try
                {
                    // LINQ query generation
                    var feedIterator = storageProvider.Queryable<TRole>()
                        .Where(role => role.NormalizedName == normalizedName)
                        .ToFeedIterator();

                    //Asynchronous query execution
                    while (feedIterator.HasMoreResults)
                    {
                        // Should only be one, so...
                        return (await feedIterator.ReadNextAsync()).FirstOrDefault();
                    }
                }
                catch (CosmosException)
                {

                }
            }

            return null;
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

            return storageProvider.CreateAsync(CreateRoleClaim(role, claim), cancellationToken);
        }

        #endregion

        #region Remove RoleClaim

        /// <summary>
        ///     Removes the given <paramref name="claim"/> from the specified <paramref name="role"/>.
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

            foreach (var roleClaim in await FindClaimsAsync(role.Id, claim, cancellationToken))
            {
                await storageProvider.DeleteAsync(roleClaim, cancellationToken);
            }
        }


        /// <summary>
        ///     Removes user claims from the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role to remove the claims from.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        protected async Task RemoveClaimsAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            foreach (var claim in await GetClaimsAsync(role, cancellationToken))
            {
                await RemoveClaimAsync(role, claim, cancellationToken);
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

        #endregion

        #region Find RoleClaims

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

        #endregion

        #endregion
    }
}
