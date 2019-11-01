// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     The interface to the identity user token persistence store.
    /// </summary>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    public interface IUserTokens<TUserToken>
        where TUserToken : IdentityUserToken, new()
    {
        /// <summary>
        ///     Adds a new user token to the store.
        /// </summary>
        /// <param name="token">The token to add.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        Task AddAsync(TUserToken token);


        /// <summary>
        ///     Removes a user token from the store.
        /// </summary>
        /// <param name="token">The token to remove.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        Task RemoveAsync(TUserToken token);


        /// <summary>
        ///     Retrieves user tokens from store if any.
        /// </summary>
        /// <param name="userId">The token owner's id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user token if it exists.</returns>
        Task<IList<TUserToken>> GetTokensAsync(string userId, CancellationToken cancellationToken);


        /// <summary>
        ///     Retrieves a user token from store if it exists.
        /// </summary>
        /// <param name="userId">The token owner's id.</param>
        /// <param name="loginProvider">The login provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user token if it exists.</returns>
        Task<TUserToken> FindAsync(string userId, string loginProvider, string name, CancellationToken cancellationToken);
    }
}
