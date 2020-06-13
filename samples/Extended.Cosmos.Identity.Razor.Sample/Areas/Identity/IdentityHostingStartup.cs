using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Extended.Cosmos.Identity.Razor.Sample.Areas.Identity.IdentityHostingStartup))]
namespace Extended.Cosmos.Identity.Razor.Sample.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}