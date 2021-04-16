using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(F1_Stats.Areas.Identity.IdentityHostingStartup))]
namespace F1_Stats.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}