// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     The interface to the identity role persistence store.
    /// </summary>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    public interface IRoles<TRole>
        where TRole : IdentityRole, new()
    {
        /// <summary>
        ///     A navigation property for the users the store contains.
        /// </summary>
        IQueryable<TRole> Queryable { get; }

        /// <summary>
        ///     Creates the specified <paramref name="role"/> in the store.
        /// </summary>
        /// <param name="role">The role to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.
        /// </returns>
        Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken);


        /// <summary>
        ///     Updates the specified <paramref name="role"/> in the store.
        /// </summary>
        /// <param name="role">The role to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.
        /// </returns>
        Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken);


        /// <summary>
        ///     Deletes the specified <paramref name="role"/> from the store.
        /// </summary>
        /// <param name="role">The role to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.
        /// </returns>
        Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken);


        /// <summary>
        ///     Finds and returns a role, if any, who has the specified <paramref name="roleId"/>.
        /// </summary>
        /// <param name="roleId">The role ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the role matching the specified <paramref name="roleId"/> if it exists.
        /// </returns>
        Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken);


        /// <summary>
        ///     Finds and returns a role, if any, who has the specified <paramref name="normalizedName"/>.
        /// </summary>
        /// <param name="normalizedName">The role to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the role matching the specified <paramref name="normalizedName"/> if it exists.
        /// </returns>
        Task<TRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken);
    }
}
