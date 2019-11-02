// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     The interface to properties that the Cosmos identity storage provider expects.
    /// </summary>
    public interface ICosmosIdentity
    {
        string Id { get; }
        string PartitionKey { get; }
    }
}
