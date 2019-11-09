using Mobsites.AspNetCore.Identity.Cosmos;

namespace Extended.Cosmos.Identity.Razor.Sample_3._0.Extensions
{
    public class ApplicationRoleClaim : IdentityRoleClaim
    {
        // Partition Key Path
        public string Discriminator => nameof(ApplicationRoleClaim);

        // Override Base property and assign correct Partition Key value.
        public override string PartitionKey => Discriminator;

        // Any other properties to extend the default Cosmos identity model
    }
}
