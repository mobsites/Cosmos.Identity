// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     The interface to the identity user role persistence store.
    /// </summary>
    /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
    public interface IUserRoles<TUserRole>
        where TUserRole : IdentityUserRole, new()
    {
        /// <summary>
        ///     Adds the given <paramref name="userRole"/> to the store.
        /// </summary>
        /// <param name="userRole">The user role to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        Task AddAsync(TUserRole userRole, CancellationToken cancellationToken);


        /// <summary>
        ///     Removes the given <paramref name="userRole"/> from the store.
        /// </summary>
        /// <param name="userRole">The user role to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        Task RemoveAsync(TUserRole userRole, CancellationToken cancellationToken);


        /// <summary>
        ///     Retrieves a list of the role names from the store that the user with the specified <paramref name="userId"/> is a member of.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The list of role names if any.</returns>
        Task<IList<string>> GetRoleNamesAsync<TUser>(string userId, CancellationToken cancellationToken)
            where TUser : IdentityUser, new();


        /// <summary>
        ///     Retrieves a list of users from the store that belong to the role with the specified <paramref name="roleId"/>.
        /// </summary>
        /// <param name="roleId">The role's id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The list of users if any.</returns>
        Task<IList<TUser>> GetUsersAsync<TUser>(string roleId, CancellationToken cancellationToken)
            where TUser : IdentityUser, new();


        /// <summary>
        ///     Retrieves a user role from the store for the given <paramref name="userId"/> and <paramref name="roleId"/> if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="roleId">The role's id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user role if it exists.</returns>
        Task<TUserRole> FindAsync(string userId, string roleId, CancellationToken cancellationToken);
    }
}
