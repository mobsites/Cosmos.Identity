// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     The interface to the identity role claim persistence store.
    /// </summary>
    /// <typeparam name="TRoleClaim">The type representing a role claim.</typeparam>
    public interface IRoleClaims<TRoleClaim>
        where TRoleClaim : IdentityRoleClaim, new()
    {
        /// <summary>
        ///     Adds the given <paramref name="roleClaim"/> to the store.
        /// </summary>
        /// <param name="roleClaim">The role claim to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        Task AddAsync(TRoleClaim roleClaim, CancellationToken cancellationToken);


        /// <summary>
        ///     Removes the given <paramref name="roleClaim"/> from the store.
        /// </summary>
        /// <param name="roleClaim">The role claim to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        Task RemoveAsync(TRoleClaim roleClaim, CancellationToken cancellationToken);


        /// <summary>
        ///     Retrieves the claims for the role with the given <paramref name="roleId"/> from the store.
        /// </summary>
        /// <param name="roleId">The id of the role to get claims for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The claims for the role if any.
        /// </returns>
        Task<IList<Claim>> GetClaimsAsync(string roleId, CancellationToken cancellationToken);


        /// <summary>
        ///     Retrieves the role claims matching the given <paramref name="claim"/> for the role with the given <paramref name="roleId"/> from the store.
        /// </summary>
        /// <param name="roleId">The id of the role to get claims for.</param>
        /// <param name="claim">The claim to match.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The matching role claims if any.
        /// </returns>
        Task<IList<TRoleClaim>> FindAsync(string roleId, Claim claim, CancellationToken cancellationToken);
    }
}
