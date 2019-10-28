// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

namespace AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     The Cosmos required implementation of an identity role which uses a string as a primary key.
    /// </summary>
    public class IdentityRole : Microsoft.AspNetCore.Identity.IdentityRole
    {
        private string id;

        /// <summary>
        ///     Override base class property so that it can be serialized correctly as primary key "id" for Cosmos.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public override string Id
        {
            get => id;
            set
            {
                id = value ?? base.Id;
            }
        }
    }
}
