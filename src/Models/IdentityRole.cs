// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Mobsites.AspNetCore.Identity.Cosmos.Models;
using System;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     The required Cosmos Identity implementation of an identity role which uses a string as a primary key.
    /// </summary>
    public class IdentityRole : Microsoft.AspNetCore.Identity.IdentityRole, ICosmosStorageType
    {
        /// <summary>
        ///     Gets the unique id associated with the item from the Azure Cosmos DB service.
        /// </summary>
        /// <remarks>
        ///     This overrides the base class property so that it can be serialized correctly as primary key "id" for Cosmos.
        /// </remarks>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public override string Id { get; set; }


        /// <summary>
        ///     Gets the partition key used by the default Cosmos storage provider.
        /// </summary>
        /// <remarks>
        ///     Override this to provide a value that is different than the default.
        /// </remarks>
        public virtual string PartitionKey => nameof(IdentityRole);


        /// <summary>
        ///     Gets the time to live in seconds of the item in the Azure Cosmos DB service.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, PropertyName = "ttl")]
        [System.Text.Json.Serialization.JsonPropertyName("ttl")]
        public int? TimeToLive { get; set; }


        /// <summary>
        ///     Gets the entity tag associated with the item from the Azure Cosmos DB service.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("_etag")]
        [System.Text.Json.Serialization.JsonPropertyName("_etag")]
        public string Etag { get; set; }


        /// <summary>
        ///     Gets the last modified timestamp associated with the item from the Azure Cosmos DB service.
        /// </summary>
        [Newtonsoft.Json.JsonConverter(typeof(UnixDateTimeConverter))]
        [Newtonsoft.Json.JsonProperty("_ts")]
        [System.Text.Json.Serialization.JsonPropertyName("_ts")]
        public DateTime Timestamp { get; set; }
    }
}
