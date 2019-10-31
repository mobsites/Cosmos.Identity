// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     The interface to the container in which to access the identity persistence store.
    /// </summary>
    public interface ICosmos
    {
        /// <summary>
        ///     The container which is used to access the identity persistence store.
        /// </summary>
        Container IdentityContainer { get; }
    }
}
