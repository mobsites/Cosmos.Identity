// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     The required Cosmos Identity implementation of an identity user role which uses a string as a primary key.
    /// </summary>
    public class IdentityUserRole : Microsoft.AspNetCore.Identity.IdentityUserRole<string>, ICosmosStorageType
    {
        /// <summary>
        ///     Initializes a new instance of <see cref="IdentityUserRole"/>.
        /// </summary>
        /// <remarks>
        ///     The Id property is initialized to form a new GUID string value.
        /// </remarks>
        public IdentityUserRole()
        {
            Id = Guid.NewGuid().ToString();
        }


        /// <summary>
        ///     Gets the unique id associated with the item from the Azure Cosmos DB service.
        /// </summary>
        /// <remarks>
        ///     Cosmos requires a string property named "id" as a primary key. 
        ///     The base class does not provide one to override or hide.
        /// </remarks>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public string Id { get; set; }


        /// <summary>
        ///     Gets the partition key used by the default Cosmos storage provider.
        /// </summary>
        /// <remarks>
        ///     Override this to provide a value that is different than the default.
        /// </remarks>
        public virtual string PartitionKey => nameof(IdentityUserRole);


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
