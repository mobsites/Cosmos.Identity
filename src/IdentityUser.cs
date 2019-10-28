using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Identity.Cosmos
{
    class IdentityUser : Microsoft.AspNetCore.Identity.IdentityUser
    {
        private string id;

        /// <summary>
        ///     Hide base class property so that it can be serialized correctly as primary key "id" for Cosmos.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public new string Id
        {
            get => id;
            set
            {
                id = value ?? base.Id;
            }
        }
    }
}
