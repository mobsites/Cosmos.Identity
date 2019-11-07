// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     Represents a new instance of a custom Cosmos storage provider which inherits <see cref="CosmosStorageProvider"/>.
    /// </summary>
    public class CustomCosmosStorageProvider : CosmosStorageProvider
    {
        public CustomCosmosStorageProvider(IConfiguration configuration) : base(configuration)
        {

        }
    }
}
