// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using System;

namespace AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     Represents a claim that a user possesses. 
    /// </summary>
    public class IdentityUserClaim : Microsoft.AspNetCore.Identity.IdentityUserClaim<string>
    {
        private string id;

        /// <summary>
        ///     Cosmos requires a string property named "id" as a primary key. 
        ///     The base class "Id" property is of type int, and so must be hidden with the new keyword.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public new string Id
        {
            get => id;
            set
            {
                id = value ?? Guid.NewGuid().ToString();
            }
        }
    }
}
