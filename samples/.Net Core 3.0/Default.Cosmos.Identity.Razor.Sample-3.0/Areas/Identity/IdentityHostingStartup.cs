using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Default.Cosmos.Identity.Razor.Sample_3._0.Areas.Identity.IdentityHostingStartup))]
namespace Default.Cosmos.Identity.Razor.Sample_3._0.Areas.Identity
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