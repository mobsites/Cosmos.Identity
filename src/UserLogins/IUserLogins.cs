// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     The interface to the identity user login persistence store.
    /// </summary>
    /// <typeparam name="TUserLogin">The type representing a user role.</typeparam>
    public interface IUserLogins<TUserLogin>
        where TUserLogin : IdentityUserLogin, new()
    {
        /// <summary>
        ///     Adds the given <paramref name="userLogin"/> to the store.
        /// </summary>
        /// <param name="userLogin">The user login to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        Task AddAsync(TUserLogin userLogin, CancellationToken cancellationToken);


        /// <summary>
        ///     Removes the given <paramref name="userLogin"/> from the store.
        /// </summary>
        /// <param name="userLogin">The user login to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        Task RemoveAsync(TUserLogin userLogin, CancellationToken cancellationToken);


        /// <summary>
        ///     Retrieves the associated logins for the user with the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The id of the user whose associated logins to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing a list of <see cref="UserLoginInfo"/> for the specified <paramref name="user"/>, if any.
        /// </returns>
        Task<IList<UserLoginInfo>> GetLoginsAsync(string userId, CancellationToken cancellationToken);


        /// <summary>
        ///     Returns a user login with the matching provider and providerKey if it exists.
        /// </summary>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        Task<TUserLogin> FindAsync(string loginProvider, string providerKey, CancellationToken cancellationToken);


        /// <summary>
        ///     Returns a user login with the matching userId, provider, and providerKey if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        Task<TUserLogin> FindAsync(string userId, string loginProvider, string providerKey, CancellationToken cancellationToken);
    }
}
