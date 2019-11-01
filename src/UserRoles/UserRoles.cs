// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     Represents a new instance of a persistence store for the identity user roles.
    /// </summary>
    /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
    public class UserRoles<TUserRole> : IUserRoles<TUserRole>
        where TUserRole : IdentityUserRole, new()
    {
        #region Setup

        private readonly ICosmos cosmos;

        /// <summary>
        ///     Constructs a new instance of <see cref="UserTokens{TUserToken}"/>.
        /// </summary>
        /// <param name="cosmos">The context in which to access the Cosmos Container for the identity store.</param>
        public UserRoles(ICosmos cosmos)
        {
            this.cosmos = cosmos ?? throw new ArgumentNullException(nameof(cosmos));
        }

        #endregion

        /// <summary>
        ///     Adds the given <paramref name="userRole"/> to the store.
        /// </summary>
        /// <param name="userRole">The user role to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async Task AddAsync(TUserRole userRole, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (userRole != null)
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(userRole.PartitionKey) ? PartitionKey.None : new PartitionKey(userRole.PartitionKey);

                    await cosmos.IdentityContainer.CreateItemAsync(userRole, partitionKey, cancellationToken: cancellationToken);
                }
                catch (CosmosException)
                {

                }
            }
        }


        /// <summary>
        ///     Removes the given <paramref name="userRole"/> from the store.
        /// </summary>
        /// <param name="userRole">The user role to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async Task RemoveAsync(TUserRole userRole, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (userRole != null)
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(userRole.PartitionKey) ? PartitionKey.None : new PartitionKey(userRole.PartitionKey);

                    await cosmos.IdentityContainer.DeleteItemAsync<TUserRole>(userRole.Id, partitionKey, cancellationToken: cancellationToken);
                }
                catch (CosmosException)
                {

                }
            }
        }


        /// <summary>
        ///     Retrieves a list of the role names from the store that the user with the specified <paramref name="userId"/> is a member of.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The list of role names if any.</returns>
        public Task<IList<string>> GetRoleNamesAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///     Retrieves a list of users from the store that belong to the role with the specified <paramref name="roleId"/>.
        /// </summary>
        /// <param name="roleId">The role's id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The list of users if any.</returns>
        public Task<IList<TUser>> GetUsersAsync<TUser>(string roleId, CancellationToken cancellationToken)
            where TUser : IdentityUser, new()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///     Retrieves a user role from the store for the given <paramref name="userId"/> and <paramref name="roleId"/> if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="roleId">The role's id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user role if it exists.</returns>
        public Task<TUserRole> FindAsync(string userId, string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
