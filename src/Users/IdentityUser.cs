// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     The Cosmos required implementation of an identity user which uses a string as a primary key.
    /// </summary>
    public class IdentityUser : Microsoft.AspNetCore.Identity.IdentityUser
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

        /// <summary>
        ///     Override this to provide a value for the partition key parameter in the Cosmos container method calls.
        ///     NOTE: The derived class must also include a property that matches the partition key path that was used when creating the container.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual string PartitionKey => null;


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
    }
}
