// Copyright (c) 2020 Allan Mobley. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Mobsites.Cosmos.Identity
{
    /// <summary>
    ///     The required Cosmos Identity implementation of an identity user which uses a string as a primary key.
    /// </summary>
    public class IdentityUser : Microsoft.AspNetCore.Identity.IdentityUser, ICosmosStorageType
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
        public virtual string PartitionKey => nameof(IdentityUser);


        /// <summary>
        ///     A comma-separated flatten list of the roles that a user has.
        /// </summary>
        /// <remarks>
        ///     Cosmos does not support document joins (that I am aware of), 
        ///     so keeping a flatten list of type string allows for using built-in Contains() query on users without the need for a join.
        /// </remarks>
        public string FlattenRoleNames { get; set; }


        /// <summary>
        ///     A comma-separated flatten list of the role ids that a user has.
        /// </summary>
        /// <remarks>
        ///     Cosmos does not support document joins (that I am aware of), 
        ///     so keeping a flatten list of type string allows for using built-in Contains() query on users without the need for a join.
        /// </remarks>
        public string FlattenRoleIds { get; set; }


        /// <summary>
        ///     A comma-separated flatten list of the claims that a user has.
        /// </summary>
        /// <remarks>
        ///     Cosmos does not support document joins (that I am aware of), 
        ///     so keeping a flatten list of type string allows for using built-in Contains() query on users without the need for a join.
        /// </remarks>
        public string FlattenClaims { get; set; }


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
