using Mobsites.AspNetCore.Identity.Cosmos;
using System;

namespace Extended.Cosmos.Identity.Razor.Sample_2._2.Extensions
{
    public class ApplicationUser : IdentityUser
    {
        // Partition Key Path
        public string Discriminator => nameof(ApplicationUser);

        // Override Base property and assign correct Partition Key value.
        public override string PartitionKey => Discriminator;

        // Any other properties to extend the default Cosmos identity model
        public DateTime TimeCreated { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
