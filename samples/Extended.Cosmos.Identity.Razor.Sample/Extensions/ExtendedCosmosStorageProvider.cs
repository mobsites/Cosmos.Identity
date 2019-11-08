// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     Represents a new instance of a custom Cosmos storage provider which inherits <see cref="CosmosStorageProvider"/>.
    /// </summary>
    public class ExtendedCosmosStorageProvider : CosmosStorageProvider
    {
        public ExtendedCosmosStorageProvider(IConfiguration configuration) : base(configuration)
        {
            // ToDo: Add members for handling other application model types not directly related to identity.
            // Since the class is register for dependency injection, it can be passed into any constructor where desired.
        }
    }
}
