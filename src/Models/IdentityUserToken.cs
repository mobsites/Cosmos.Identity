// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     Represents an authentication token for a user.
    /// </summary>
    public class IdentityUserToken : Microsoft.AspNetCore.Identity.IdentityUserToken<string>, ICosmosIdentity
    {
        private string id;

        /// <summary>
        ///     Cosmos requires a string property named "id" as a primary key. 
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public string Id
        {
            get => id;
            set
            {
                id = string.IsNullOrEmpty(value) ? Guid.NewGuid().ToString() : value;
            }
        }

        /// <summary>
        ///     Override this to provide a value for the partition key parameter in the Cosmos container method calls.
        ///     NOTE: The derived class must also include a property that matches the partition key path that was used when creating the container.
        /// </summary>
        public virtual string PartitionKey => nameof(IdentityUserToken);
    }
}
