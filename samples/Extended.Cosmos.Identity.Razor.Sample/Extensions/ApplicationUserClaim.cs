using Mobsites.Cosmos.Identity;

namespace Extended.Cosmos.Identity.Razor.Sample.Extensions
{
    public class ApplicationUserClaim : IdentityUserClaim
    {
        // Partition Key Path
        public string Discriminator => nameof(ApplicationUserClaim);

        // Override Base property and assign correct Partition Key value.
        public override string PartitionKey => Discriminator;

        // Any other properties to extend the default Cosmos identity model
    }
}
