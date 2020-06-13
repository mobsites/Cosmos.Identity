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

namespace Mobsites.Cosmos.Identity
{
    /// <summary>
    ///     Represents a new instance of a persistence store for the specified types.
    /// </summary>
    /// <typeparam name="TCustomStorageProvider">The type representing a custom storage provider.</typeparam>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a user claim.</typeparam>
    /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    /// <typeparam name="TRoleClaim">The type representing a role claim.</typeparam>
    public class UserStore<TCustomStorageProvider, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> :
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
        #region Setup

        private readonly TCustomStorageProvider storageProvider;

        /// <summary>
        ///     Constructs a new instance of <see cref="UserStore{TCustomStorageProvider, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim}"/>.
        /// </summary>
        /// <param name="storageProvider">The provider used to access the store.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe store errors.</param>
        public UserStore(TCustomStorageProvider storageProvider, IdentityErrorDescriber describer = null) 
            : base(describer ?? new IdentityErrorDescriber())
        {
            this.storageProvider = storageProvider ?? throw new ArgumentNullException(nameof(storageProvider));
        }

        #endregion

        #region User Store

        /// <summary>
        ///     A navigation property for the users the store contains.
        /// </summary>
        public override IQueryable<TUser> Users => storageProvider.Queryable<TUser>();

        #region Create User

        /// <summary>
        ///     Creates the specified <paramref name="user"/> in the store.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.
        /// </returns>
        public override Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return storageProvider.CreateAsync(user, cancellationToken);
        }

        #endregion

        #region Update User

        /// <summary>
        ///     Updates the specified <paramref name="user"/> in the store.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.
        /// </returns>
        public override Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.ConcurrencyStamp = Guid.NewGuid().ToString();

            return storageProvider.UpdateAsync(user, cancellationToken);
        }

        #endregion

        #region Delete User

        /// <summary>
        ///     Deletes the specified <paramref name="user"/> from the store.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.
        /// </returns>
        public async override Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var result = await storageProvider.DeleteAsync(user, cancellationToken);

            if (result.Succeeded)
            {
                // Cosmos does not know about foreign key relationships, so...
                await RemoveFromRolesAsync(user, cancellationToken);
                await RemoveClaimsAsync(user, cancellationToken);
                await RemoveLoginsAsync(user, cancellationToken);
                await RemoveUserTokensAsync(user, cancellationToken);
            }

            return result;
        }

        #endregion

        #region Find User

        /// <summary>
        ///     Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId"/> if it exists.
        /// </returns>
        protected override Task<TUser> FindUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return storageProvider.FindByIdAsync<TUser>(userId, cancellationToken);
        }


        /// <summary>
        ///     Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId"/> if it exists.
        /// </returns>
        public override Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return storageProvider.FindByIdAsync<TUser>(userId, cancellationToken);
        }


        /// <summary>
        ///     Finds and returns a user, if any, who has the specified <paramref name="normalizedUserName"/>.
        /// </summary>
        /// <param name="normalizedUserName">The normalized user name to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedUserName"/> if it exists.
        /// </returns>
        public async override Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (!string.IsNullOrEmpty(normalizedUserName))
            {
                try
                {
                    // LINQ query generation
                    var feedIterator = storageProvider.Queryable<TUser>()
                        .Where(user => user.NormalizedUserName == normalizedUserName)
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


        /// <summary>
        ///     Finds and returns a user, if any, who has the specified <paramref name="normalizedEmail"/>.
        /// </summary>
        /// <param name="normalizedEmail">The normalized email address to return the user for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///      The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedEmail"/> if it exists.
        /// </returns>
        public async override Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (!string.IsNullOrEmpty(normalizedEmail))
            {
                try
                {
                    // LINQ query generation
                    var feedIterator = storageProvider.Queryable<TUser>()
                        .Where(user => user.NormalizedEmail == normalizedEmail)
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

        #region Role Store

        #region Find Role

        /// <summary>
        ///    Return a role from the store with the given <paramref name="normalizedRoleName"/> if it exists.
        /// </summary>
        /// <param name="normalizedRoleName">The normalized role name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The role if it exists.</returns>
        protected async override Task<TRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (!string.IsNullOrEmpty(normalizedRoleName))
            {
                try
                {
                    var partitionKey = new TRole().PartitionKey;

                    // LINQ query generation
                    var feedIterator = storageProvider.Queryable<TRole>()
                        .Where(role => role.NormalizedName == normalizedRoleName)
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

        #region UserRole Store

        #region Add UserRole

        /// <summary>
        ///     Adds the given <paramref name="normalizedRoleName"/> to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to add the role to.</param>
        /// <param name="normalizedRoleName">The role to add to.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async override Task AddToRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default)
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
            if (role is null)
            {
                throw new InvalidOperationException($"{normalizedRoleName} does not exist.");
            }
            
            var result = await storageProvider.CreateAsync(CreateUserRole(user, role), cancellationToken);

            if (result.Succeeded)
            {
                // Update user object (default UserManager will actually call UpdateUserAsync(user).
                user.FlattenRoleNames += role.Name + ",";
                user.FlattenRoleIds += role.Id + ",";
            }
        }

        #endregion

        #region Remove UserRole

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
                            user.FlattenRoleNames.Replace(role.Name + ",", string.Empty);
                        }
                        if (!string.IsNullOrEmpty(user.FlattenRoleIds))
                        {
                            user.FlattenRoleIds.Replace(role.Id + ",", string.Empty);
                        }
                    }
                }
            }
        }


        /// <summary>
        ///     Removes user roles from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the roles from.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        protected async Task RemoveFromRolesAsync(TUser user,  CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            foreach (var roleName in await GetRolesAsync(user, cancellationToken) ?? new List<string>())
            {
                await RemoveFromRoleAsync(user, roleName.Normalize().ToUpperInvariant(), cancellationToken);
            }
        }

        #endregion

        #region Get UserRoles

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

        #endregion

        #region Has UserRole

        /// <summary>
        ///     Returns a flag indicating if the specified user is a member of the give <paramref name="normalizedRoleName"/>.
        /// </summary>
        /// <param name="user">The user whose role membership should be checked.</param>
        /// <param name="normalizedRoleName">The role to check membership of</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     A <see cref="Task{TResult}"/> containing a flag indicating if the specified user is a member of the given group.
        /// </returns>
        public async override Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default)
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

                return userRole != null;
            }

            return false;
        }

        #endregion

        #region Find UserRole

        /// <summary>
        ///     Return a user role for the userId and roleId if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="roleId">The role's id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user role if it exists.</returns>
        protected async override Task<TUserRole> FindUserRoleAsync(string userId, string roleId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            try
            {
                // LINQ query generation
                var feedIterator = storageProvider.Queryable<TUserRole>()
                    .Where(userRole => userRole.UserId == userId && userRole.RoleId == roleId)
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

            return null;
        }

        #endregion

        #region Get Role Users

        /// <summary>
        ///     Retrieves all users in the specified role.
        /// </summary>
        /// <param name="normalizedRoleName">The role whose users should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> contains a list of users, if any, that are in the specified role.
        /// </returns>
        public async override Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            IList<TUser> users = new List<TUser>();

            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(normalizedRoleName))
            {
                throw new ArgumentException($"{nameof(normalizedRoleName)} cannot be null or empty.");
            }

            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (role != null)
            {
                try
                {
                    // LINQ query generation
                    var feedIterator = storageProvider.Queryable<TUser>()
                        .Where(user => user.FlattenRoleIds.Contains(role.Id))
                        .ToFeedIterator();

                    //Asynchronous query execution
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var user in await feedIterator.ReadNextAsync())
                        {
                            users.Add(user);
                        }
                    }
                }
                catch (CosmosException)
                {

                }
            }

            return users;
        }

        #endregion

        #endregion

        #region UserClaim Store

        #region Add UserClaim

        /// <summary>
        ///     Adds the given <paramref name="claims"/> to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to add the claims to.</param>
        /// <param name="claims">The claims to add to the user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async override Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (claims is null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            foreach (var claim in claims)
            {
                var result = await storageProvider.CreateAsync(CreateUserClaim(user, claim), cancellationToken);

                if (result.Succeeded)
                {
                    // Update user object (default UserManager will actually call UpdateUserAsync(user).
                    user.FlattenClaims += $"{claim.Type}|{claim.Value},";
                }
            }
        }

        #endregion

        #region Replace UserClaim

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

        #endregion

        #region Remove UserClaim

        /// <summary>
        ///     Removes the given <paramref name="claims"/> from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the claims from.</param>
        /// <param name="claims">The claims to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async override Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (claims is null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            foreach (var claim in claims)
            {
                foreach (var userClaim in await FindClaimsAsync(user.Id, claim, cancellationToken))
                {
                    var result = await storageProvider.DeleteAsync(userClaim, cancellationToken);

                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(user.FlattenClaims))
                        {
                            user.FlattenClaims.Replace($"{userClaim.ClaimType}|{userClaim.ClaimValue},", string.Empty);
                        }
                    }
                }
            }
        }


        /// <summary>
        ///     Removes user claims from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the claims from.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        protected async Task RemoveClaimsAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            
            await RemoveClaimsAsync(user, await GetClaimsAsync(user, cancellationToken), cancellationToken);
        }

        #endregion

        #region Get UserClaims

        /// <summary>
        ///     Retrieves the claims for the given <paramref name="user"/> from the store.
        /// </summary>
        /// <param name="user">The user to get claims for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The claims for the user if any.
        /// </returns>
        public async override Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = default)
        {
            IList<Claim> claims = new List<Claim>();

            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                // LINQ query generation
                var feedIterator = storageProvider.Queryable<TUserClaim>()
                    .Where(userClaim => userClaim.UserId == user.Id)
                    .ToFeedIterator();

                //Asynchronous query execution
                while (feedIterator.HasMoreResults)
                {
                    foreach (var userClaim in await feedIterator.ReadNextAsync())
                    {
                        claims.Add(userClaim.ToClaim());
                    }
                }
            }
            catch (CosmosException)
            {

            }

            return claims;
        }

        #endregion

        #region Get Claim Users

        public async override Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default)
        {
            IList<TUser> users = new List<TUser>();

            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (claim is null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            try
            {
                // LINQ query generation
                var feedIterator = storageProvider.Queryable<TUser>()
                    .Where(user => user.FlattenClaims.Contains($"{claim.Type}|{claim.Value}"))
                    .ToFeedIterator();

                //Asynchronous query execution
                while (feedIterator.HasMoreResults)
                {
                    foreach (var user in await feedIterator.ReadNextAsync())
                    {
                        users.Add(user);
                    }
                }
            }
            catch (CosmosException)
            {

            }

            return users;
        }

        #endregion

        #region Find UserClaims

        /// <summary>
        ///     Retrieves the user claims matching the given <paramref name="claim"/> for the user with the given <paramref name="userId"/> from the store.
        /// </summary>
        /// <param name="userId">The id of the user to get claims for.</param>
        /// <param name="claim">The claim to match.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The matching user claims if any.
        /// </returns>
        public async Task<IList<TUserClaim>> FindClaimsAsync(string userId, Claim claim, CancellationToken cancellationToken)
        {
            IList<TUserClaim> userClaims = new List<TUserClaim>();

            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    // LINQ query generation
                    var feedIterator = storageProvider.Queryable<TUserClaim>()
                        .Where(userClaim => userClaim.UserId == userId && userClaim.ClaimType == claim.Type && userClaim.ClaimValue == claim.Value)
                        .ToFeedIterator();

                    //Asynchronous query execution
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var userClaim in await feedIterator.ReadNextAsync())
                        {
                            userClaims.Add(userClaim);
                        }
                    }
                }
                catch (CosmosException)
                {

                }
            }

            return userClaims;
        }

        #endregion

        #endregion

        #region UserLogin Store

        #region Add UserLogin

        /// <summary>
        ///     Adds the given <paramref name="login"/> to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to add the login to.</param>
        /// <param name="login">The login to add to the user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (login is null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            return storageProvider.CreateAsync(CreateUserLogin(user, login), cancellationToken);
        }

        #endregion

        #region Remove UserLogin

        /// <summary>
        ///     Removes the <paramref name="loginProvider"/> given from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the login from.</param>
        /// <param name="loginProvider">The login to remove from the user.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async override Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrEmpty(loginProvider))
            {
                throw new ArgumentException($"{nameof(loginProvider)} cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(providerKey))
            {
                throw new ArgumentException($"{nameof(providerKey)} cannot be null or empty.");
            }

            var userLogin = await FindUserLoginAsync(user.Id, loginProvider, providerKey, cancellationToken);
            if (userLogin != null)
            {
                await storageProvider.DeleteAsync(userLogin, cancellationToken);
            }
        }


        /// <summary>
        ///     Removes users logins from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the logins from.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        protected async Task RemoveLoginsAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            foreach (var userLoginInfo in await GetLoginsAsync(user, cancellationToken))
            {
                await RemoveLoginAsync(user, userLoginInfo.LoginProvider, userLoginInfo.ProviderKey, cancellationToken);
            }
        }

        #endregion

        #region Get UserLogins

        /// <summary>
        ///     Retrieves the associated logins for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose associated logins to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing a list of <see cref="UserLoginInfo"/> for the specified <paramref name="user"/>, if any.
        /// </returns>
        public async override Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default)
        {
            IList<UserLoginInfo> userLognins = new List<UserLoginInfo>();

            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                // LINQ query generation
                var feedIterator = storageProvider.Queryable<TUserLogin>()
                    .Where(userLogin => userLogin.UserId == user.Id)
                    .ToFeedIterator();

                //Asynchronous query execution
                while (feedIterator.HasMoreResults)
                {
                    foreach (var userLogin in await feedIterator.ReadNextAsync())
                    {
                        userLognins.Add(new UserLoginInfo(userLogin.LoginProvider, userLogin.ProviderKey, userLogin.ProviderDisplayName));
                    }
                }
            }
            catch (CosmosException)
            {

            }

            return userLognins;
        }

        #endregion

        #region Find UserLogin

        /// <summary>
        ///     Returns a user login with  provider, providerKey if it exists.
        /// </summary>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        protected async override Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(loginProvider))
            {
                throw new ArgumentException($"{nameof(loginProvider)} cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(providerKey))
            {
                throw new ArgumentException($"{nameof(providerKey)} cannot be null or empty.");
            }

            try
            {
                // LINQ query generation
                var feedIterator = storageProvider.Queryable<TUserLogin>()
                    .Where(userLogin => userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey)
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

            return null;
        }


        /// <summary>
        ///     Returns a user login with the matching userId, provider, providerKey if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        protected async override Task<TUserLogin> FindUserLoginAsync(string userId, string loginProvider, string providerKey, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException($"{nameof(userId)} cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(loginProvider))
            {
                throw new ArgumentException($"{nameof(loginProvider)} cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(providerKey))
            {
                throw new ArgumentException($"{nameof(providerKey)} cannot be null or empty.");
            }

            try
            {
                // LINQ query generation
                var feedIterator = storageProvider.Queryable<TUserLogin>()
                    .Where(userLogin => userLogin.UserId == userId && userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey)
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

            return null;
        }

        #endregion

        #endregion

        #region UserToken Store

        #region Add UserToken

        /// <summary>
        ///     Adds a new user token to the store.
        /// </summary>
        /// <param name="token">The token to add.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        protected override Task AddUserTokenAsync(TUserToken token)
        {
            ThrowIfDisposed();

            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return storageProvider.CreateAsync(token);
        }

        #endregion

        #region Remove UserToken

        /// <summary>
        ///     Removes a user token from the store.
        /// </summary>
        /// <param name="token">The token to remove.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        protected override Task RemoveUserTokenAsync(TUserToken token)
        {
            ThrowIfDisposed();

            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return storageProvider.DeleteAsync(token);
        }


        /// <summary>
        ///     Removes all user tokens from the store.
        /// </summary>
        /// <param name="token">The token to remove.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        protected async Task RemoveUserTokensAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            foreach (var userToken in await GetTokensAsync(user.Id, cancellationToken))
            {
                await storageProvider.DeleteAsync(userToken, cancellationToken);
            }
        }

        #endregion

        #region Get UserTokens

        /// <summary>
        ///     Retrieves user tokens from store if any.
        /// </summary>
        /// <param name="userId">The token owner's id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user token if it exists.</returns>
        public async Task<IList<TUserToken>> GetTokensAsync(string userId, CancellationToken cancellationToken)
        {
            IList<TUserToken> userTokens = new List<TUserToken>();

            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    // LINQ query generation
                    var feedIterator = storageProvider.Queryable<TUserToken>()
                        .Where(userToken => userToken.UserId == userId)
                        .ToFeedIterator();

                    //Asynchronous query execution
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var userToken in await feedIterator.ReadNextAsync())
                        {
                            userTokens.Add(userToken);
                        }
                    }
                }
                catch (CosmosException)
                {

                }
            }

            return userTokens;
        }

        #endregion

        #region Find UserToken

        /// <summary>
        ///     Retrieves a user token from store if it exists.
        /// </summary>
        /// <param name="user">The token owner.</param>
        /// <param name="loginProvider">The login provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user token if it exists.</returns>
        protected async override Task<TUserToken> FindTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrEmpty(loginProvider))
            {
                throw new ArgumentException($"{nameof(loginProvider)} cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"{nameof(name)} cannot be null or empty.");
            }

            try
            {
                // LINQ query generation
                var feedIterator = storageProvider.Queryable<TUserToken>()
                    .Where(userToken => userToken.UserId == user.Id && userToken.LoginProvider == loginProvider && userToken.Name == name)
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

            return null;
        }

        #endregion

        #endregion
    }
}
