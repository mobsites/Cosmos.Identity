// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     The interface to the identity user claim persistence store.
    /// </summary>
    /// <typeparam name="TUserClaim">The type representing a user claim.</typeparam>
    public interface IUserClaims<TUserClaim>
        where TUserClaim : IdentityUserClaim, new()
    {
        /// <summary>
        ///     Adds the given <paramref name="userClaim"/> to the store.
        /// </summary>
        /// <param name="userClaim">The user claim to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        Task AddAsync(TUserClaim userClaim, CancellationToken cancellationToken);


        /// <summary>
        ///     Updates the given <paramref name="userClaim"/> in the store.
        /// </summary>
        /// <param name="userClaim">The user claim to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        Task UpdateAsync(TUserClaim userClaim, CancellationToken cancellationToken);


        /// <summary>
        ///     Removes the given <paramref name="userClaim"/> from the store.
        /// </summary>
        /// <param name="userClaim">The user claim to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        Task RemoveAsync(TUserClaim userClaim, CancellationToken cancellationToken);


        /// <summary>
        ///     Retrieves the claims for the user with the given <paramref name="userId"/> from the store.
        /// </summary>
        /// <param name="userId">The id of the user to get claims for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The claims for the user if any.
        /// </returns>
        Task<IList<Claim>> GetClaimsAsync(string userId, CancellationToken cancellationToken);


        /// <summary>
        ///     Retrieves a list of users from the store that have the specified <paramref name="claim"/>.
        /// </summary>
        /// <param name="claim">The claim to match to users.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The list of users if any.</returns>
        Task<IList<TUser>> GetUsersAsync<TUser>(Claim claim, CancellationToken cancellationToken)
            where TUser : IdentityUser, new();


        /// <summary>
        ///     Retrieves the user claims matching the given <paramref name="claim"/> for the user with the given <paramref name="userId"/> from the store.
        /// </summary>
        /// <param name="userId">The id of the user to get claims for.</param>
        /// <param name="claim">The claim to match.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The matching user claims if any.
        /// </returns>
        Task<IList<TUserClaim>> FindAsync(string userId, Claim claim, CancellationToken cancellationToken);
    }
}
