// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     The required Cosmos Identity implementation of an identity user token which uses a string as a primary key.
    /// </summary>
    public class IdentityUserToken : Microsoft.AspNetCore.Identity.IdentityUserToken<string>, ICosmosStorageType
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IdentityUserToken"/>.
        /// </summary>
        /// <remarks>
        /// The Id property is initialized to form a new GUID string value.
        /// </remarks>
        public IdentityUserToken()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        ///     Cosmos requires a string property named "id" as a primary key. 
        ///     The base class does not provide one to override or hide.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        ///     Override this to provide a value for the partition key which is user by the default Cosmos storage provider's members.
        ///     NOTE: The derived class must also include a property that matches the partition key path that was used when creating the container.
        /// </summary>
        public virtual string PartitionKey => nameof(IdentityUserToken);
    }
}
