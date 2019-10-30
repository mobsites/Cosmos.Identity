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
    ///     Represents a new instance of a persistence store for users, using the default Cosmos no-partition key implementation.
    /// </summary>
    public class UserStore : UserStore<IdentityUser>
    {
        /// <summary>
        ///     Constructs a new instance of <see cref="UserStore"/>.
        /// </summary>
        /// <param name="users">The context used to access the user store.</param>
        /// <param name="roles">The context used to access the role store.</param>
        /// <param name="userRoles">The context used to access the userRole store.</param>
        /// <param name="userClaims">The context used to access the userClaim store.</param>
        /// <param name="userLogins">The context used to access the userLogin store.</param>
        /// <param name="userTokens">The context used to access the userToken store.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe store errors.</param>
        public UserStore(
            IUsers<IdentityUser> users,
            IRoles<IdentityRole> roles,
            IUserRoles<IdentityUserRole> userRoles,
            IUserClaims<IdentityUserClaim> userClaims,
            IUserLogins<IdentityUserLogin> userLogins,
            IUserTokens<IdentityUserToken> userTokens,
            IdentityErrorDescriber describer = null) : base(users, roles, userRoles, userClaims, userLogins, userTokens, describer) { }
    }


    /// <summary>
    ///     Represents a new instance of a persistence store for the specified user type.
    /// </summary>
    /// /// <typeparam name="TUser">The type representing a user.</typeparam>
    public class UserStore<TUser> : UserStore<TUser, IdentityRole>
        where TUser : IdentityUser, new()
    {
        /// <summary>
        ///     Constructs a new instance of <see cref="UserStore{TUser}"/>.
        /// </summary>
        /// <param name="users">The context used to access the user store.</param>
        /// <param name="roles">The context used to access the role store.</param>
        /// <param name="userRoles">The context used to access the userRole store.</param>
        /// <param name="userClaims">The context used to access the userClaim store.</param>
        /// <param name="userLogins">The context used to access the userLogin store.</param>
        /// <param name="userTokens">The context used to access the userToken store.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe store errors.</param>
        public UserStore(
            IUsers<TUser> users,
            IRoles<IdentityRole> roles,
            IUserRoles<IdentityUserRole> userRoles,
            IUserClaims<IdentityUserClaim> userClaims,
            IUserLogins<IdentityUserLogin> userLogins,
            IUserTokens<IdentityUserToken> userTokens,
            IdentityErrorDescriber describer = null) : base(users, roles, userRoles, userClaims, userLogins, userTokens, describer) { }
    }


    /// <summary>
    ///     Represents a new instance of a persistence store for the specified user and role types.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    public class UserStore<TUser, TRole> : UserStore<TUser, TRole, IdentityUserClaim, IdentityUserRole, IdentityUserLogin, IdentityUserToken, IdentityRoleClaim>
        where TUser : IdentityUser, new()
        where TRole : IdentityRole, new()
    {
        /// <summary>
        ///     Constructs a new instance of <see cref="UserStore{TUser, TRole}"/>.
        /// </summary>
        /// <param name="users">The context used to access the user store.</param>
        /// <param name="roles">The context used to access the role store.</param>
        /// <param name="userRoles">The context used to access the userRole store.</param>
        /// <param name="userClaims">The context used to access the userClaim store.</param>
        /// <param name="userLogins">The context used to access the userLogin store.</param>
        /// <param name="userTokens">The context used to access the userToken store.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe store errors.</param>
        public UserStore(
            IUsers<TUser> users,
            IRoles<TRole> roles,
            IUserRoles<IdentityUserRole> userRoles,
            IUserClaims<IdentityUserClaim> userClaims,
            IUserLogins<IdentityUserLogin> userLogins,
            IUserTokens<IdentityUserToken> userTokens,
            IdentityErrorDescriber describer = null) : base(users, roles, userRoles, userClaims, userLogins, userTokens, describer) { }
    }


    /// <summary>
    ///     Represents a new instance of a persistence store for the specified types.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
    /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    /// <typeparam name="TRoleClaim">The type representing a role claim.</typeparam>
    public class UserStore<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> :
        UserStoreBase<TUser, TRole, string, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>,
        IProtectedUserStore<TUser>
        where TUser : IdentityUser, new()
        where TRole : IdentityRole, new()
        where TUserClaim : IdentityUserClaim, new()
        where TUserRole : IdentityUserRole, new()
        where TUserLogin : IdentityUserLogin, new()
        where TUserToken : IdentityUserToken, new()
        where TRoleClaim : IdentityRoleClaim, new()
    {
        #region Setup

        private readonly IUsers<TUser> users;
        private readonly IRoles<TRole> roles;
        private readonly IUserRoles<TUserRole> userRoles;
        private readonly IUserClaims<TUserClaim> userClaims;
        private readonly IUserLogins<TUserLogin> userLogins;
        private readonly IUserTokens<TUserToken> userTokens;

        /// <summary>
        ///     Constructs a new instance of <see cref="UserStore{TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim}"/>.
        /// </summary>
        /// <param name="users">The context used to access the user store.</param>
        /// <param name="roles">The context used to access the role store.</param>
        /// <param name="userRoles">The context used to access the userRole store.</param>
        /// <param name="userClaims">The context used to access the userClaim store.</param>
        /// <param name="userLogins">The context used to access the userLogin store.</param>
        /// <param name="userTokens">The context used to access the userToken store.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe store errors.</param>
        public UserStore(
            IUsers<TUser> users,
            IRoles<TRole> roles,
            IUserRoles<TUserRole> userRoles,
            IUserClaims<TUserClaim> userClaims,
            IUserLogins<TUserLogin> userLogins,
            IUserTokens<TUserToken> userTokens,
            IdentityErrorDescriber describer = null) : base(describer ?? new IdentityErrorDescriber())
        {
            this.users = users ?? throw new ArgumentNullException(nameof(users));
            this.roles = roles ?? throw new ArgumentNullException(nameof(roles));
            this.userRoles = userRoles ?? throw new ArgumentNullException(nameof(userRoles));
            this.userClaims = userClaims ?? throw new ArgumentNullException(nameof(userClaims));
            this.userLogins = userLogins ?? throw new ArgumentNullException(nameof(userLogins));
            this.userTokens = userTokens ?? throw new ArgumentNullException(nameof(userTokens));
        }

        #endregion

        #region User Store

        #region Users

        /// <summary>
        ///     A navigation property for the users the store contains.
        /// </summary>
        public override IQueryable<TUser> Users => users.Queryable;

        #endregion

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

            return users.CreateAsync(user, cancellationToken);
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

            return users.UpdateAsync(user, cancellationToken);
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

            var result = await users.DeleteAsync(user, cancellationToken);

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

            return users.FindByIdAsync(userId, cancellationToken);
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

            return users.FindByIdAsync(userId, cancellationToken);
        }


        /// <summary>
        ///     Finds and returns a user, if any, who has the specified <paramref name="normalizedUserName"/>.
        /// </summary>
        /// <param name="normalizedUserName">The normalized user name to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedUserName"/> if it exists.
        /// </returns>
        public override Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return users.FindByNameAsync(normalizedUserName, cancellationToken);
        }


        /// <summary>
        ///     Finds and returns a user, if any, who has the specified <paramref name="normalizedEmail"/>.
        /// </summary>
        /// <param name="normalizedEmail">The normalized email address to return the user for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///      The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedEmail"/> if it exists.
        /// </returns>
        public override Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return users.FindByEmailAsync(normalizedEmail, cancellationToken);
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
        protected override Task<TRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return roles.FindByNameAsync(normalizedRoleName, cancellationToken);
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
            
            await userRoles.AddAsync(CreateUserRole(user, role), cancellationToken);
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
                    await userRoles.RemoveAsync(userRole, cancellationToken);
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
        ///     Retrieves the roles the specified <paramref name="user"/> is a member of.
        /// </summary>
        /// <param name="user">The user whose roles should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     A <see cref="Task{TResult}"/> that contains the roles the user is a member of.
        /// </returns>
        public override Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return userRoles.GetRoleNamesAsync(user.Id, cancellationToken);
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
        protected override Task<TUserRole> FindUserRoleAsync(string userId, string roleId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return userRoles.FindAsync(userId, roleId, cancellationToken);
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
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(normalizedRoleName))
            {
                throw new ArgumentException($"{nameof(normalizedRoleName)} cannot be null or empty.");
            }

            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (role != null)
            {
                return await userRoles.GetUsersAsync<TUser>(normalizedRoleName, cancellationToken);
            }

            return new List<TUser>();
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
                await userClaims.AddAsync(CreateUserClaim(user, claim), cancellationToken);
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

            var userClaimMatches = await userClaims.FindAsync(user.Id, claim, cancellationToken);
            if (userClaimMatches != null)
            {
                foreach (var userClaim in userClaimMatches)
                {
                    userClaim.ClaimType = newClaim.Type;
                    userClaim.ClaimValue = newClaim.Value;

                    await userClaims.UpdateAsync(userClaim, cancellationToken);
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
                var userClaimMatches = await userClaims.FindAsync(user.Id, claim, cancellationToken);
                if (userClaimMatches != null)
                {
                    foreach (var userClaim in userClaimMatches)
                    {
                        await userClaims.RemoveAsync(userClaim, cancellationToken);
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
            
            await RemoveClaimsAsync(user, await GetClaimsAsync(user, cancellationToken) ?? new List<Claim>(), cancellationToken);
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
        public override Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return userClaims.GetClaimsAsync(user.Id, cancellationToken);
        }

        #endregion

        #region Get Users With Claim

        public override Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (claim is null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            return userClaims.GetUsersAsync<TUser>(claim, cancellationToken);
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

            return userLogins.AddAsync(CreateUserLogin(user, login), cancellationToken);
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
                await userLogins.RemoveAsync(userLogin, cancellationToken);
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

            foreach (var userLoginInfo in await GetLoginsAsync(user, cancellationToken) ?? new List<UserLoginInfo>())
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
        public override Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return userLogins.GetLoginsAsync(user.Id, cancellationToken);
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
        protected override Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default)
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

            return userLogins.FindAsync(loginProvider, providerKey, cancellationToken);
        }


        /// <summary>
        ///     Returns a user login with the matching userId, provider, providerKey if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        protected override Task<TUserLogin> FindUserLoginAsync(string userId, string loginProvider, string providerKey, CancellationToken cancellationToken = default)
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

            return userLogins.FindAsync(userId, loginProvider, providerKey, cancellationToken);
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

            return userTokens.AddAsync(token);
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

            return userTokens.RemoveAsync(token);
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

            foreach (var token in await userTokens.GetTokensAsync(user.Id, cancellationToken) ?? new List<TUserToken>())
            {
                await userTokens.RemoveAsync(token);
            }
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
        protected override Task<TUserToken> FindTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken = default)
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

            return userTokens.FindAsync(user.Id, loginProvider, name, cancellationToken);
        }

        #endregion

        #endregion
    }
}
