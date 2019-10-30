// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    public interface IUsers<TUser>
        where TUser : IdentityUser
    {
        /// <summary>
        ///     A navigation property for the users the store contains.
        /// </summary>
        IQueryable<TUser> Queryable { get; }


        /// <summary>
        ///     Creates the specified <paramref name="user"/> in the store.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.
        /// </returns>
        Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken);


        /// <summary>
        ///     Updates the specified <paramref name="user"/> in the store.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.
        /// </returns>
        Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken);


        /// <summary>
        ///     Deletes the specified <paramref name="user"/> from the store.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.
        /// </returns>
        Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken);


        /// <summary>
        ///     Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId"/> if it exists.
        /// </returns>
        Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken);


        /// <summary>
        ///     Finds and returns a user, if any, who has the specified <paramref name="normalizedUserName"/>.
        /// </summary>
        /// <param name="normalizedUserName">The normalized user name to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedUserName"/> if it exists.
        /// </returns>
        Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken);


        /// <summary>
        ///     Finds and returns a user, if any, who has the specified <paramref name="normalizedEmail"/>.
        /// </summary>
        /// <param name="normalizedEmail">The normalized email address to return the user for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///      The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedEmail"/> if it exists.
        /// </returns>
        Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);
    }

}
