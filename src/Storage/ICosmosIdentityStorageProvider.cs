// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     The interface to the Cosmos identity storage provider.
    /// </summary>
    public interface ICosmosIdentityStorageProvider
    {
        /// <summary>
        ///     Returns a queryable linq expression of the specified <typeparam name="TIdentity"/>.
        /// </summary>
        IOrderedQueryable<TIdentity> Queryable<TIdentity>()
            where TIdentity : ICosmosIdentity, new();


        /// <summary>
        ///     Creates the specified <paramref name="identityModel"/> in the store.
        /// </summary>
        /// <param name="identityModel">The identity model to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.
        /// </returns>
        Task<IdentityResult> CreateAsync<TIdentity>(TIdentity identityModel, CancellationToken cancellationToken = default)
            where TIdentity : ICosmosIdentity, new();


        /// <summary>
        ///     Updates the specified <paramref name="identityModel"/> in the store.
        /// </summary>
        /// <param name="identityModel">The identity model to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.
        /// </returns>
        Task<IdentityResult> UpdateAsync<TIdentity>(TIdentity identityModel, CancellationToken cancellationToken = default)
            where TIdentity : ICosmosIdentity, new();


        /// <summary>
        ///     Deletes the specified <paramref name="identityModel"/> from the store.
        /// </summary>
        /// <param name="identityModel">The identity model to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.
        /// </returns>
        Task<IdentityResult> DeleteAsync<TIdentity>(TIdentity identityModel, CancellationToken cancellationToken = default)
            where TIdentity : ICosmosIdentity, new();


        /// <summary>
        ///     Finds and returns an identity model, if any, who has the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the identity model to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the identity model with the specified <paramref name="id"/> if it exists.
        /// </returns>
        Task<TIdentity> FindByIdAsync<TIdentity>(string id, CancellationToken cancellationToken = default)
            where TIdentity : ICosmosIdentity, new();
    }
}
