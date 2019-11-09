// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     The required Cosmos Identity implementation of an identity role which uses a string as a primary key.
    /// </summary>
    public class IdentityRole : Microsoft.AspNetCore.Identity.IdentityRole, ICosmosStorageType
    {
        /// <summary>
        ///     Override base class property so that it can be serialized correctly as primary key "id" for Cosmos.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public override string Id { get; set; }

        /// <summary>
        ///     Override this to provide a value for the partition key which is user by the default Cosmos storage provider's members.
        ///     NOTE: The derived class must also include a property that matches the partition key path that was used when creating the container.
        /// </summary>
        public virtual string PartitionKey => nameof(IdentityRole);
    }
}
