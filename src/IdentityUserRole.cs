// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using System;

namespace AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     Represents the link between a user and a role.
    /// </summary>
    public class IdentityUserRole : Microsoft.AspNetCore.Identity.IdentityUserRole<string>
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
                id = value ?? Guid.NewGuid().ToString();
            }
        }
    }
}
