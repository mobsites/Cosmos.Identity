using Mobsites.AspNetCore.Identity.Cosmos;

namespace Extended.Cosmos.Identity.Razor.Sample.Extensions
{
    public class ApplicationRole : IdentityRole
    {
        // Partition Key Path
        public string Discriminator => nameof(ApplicationRole);

        // Override Base property and assign correct Partition Key value.
        public override string PartitionKey => Discriminator;

        // Any other properties to extend the default Cosmos identity user
    }
}
