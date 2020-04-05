using DeviceManager.Api.UnitTests.Api.Server.Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceManager.Api.UnitTests.Api.Server
{
    public class TestStartup : Startup
    {
        public TestStartup(IHostingEnvironment env, IConfiguration configuration)
            : base(env, configuration)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            TestIocContainerConfiguration.ConfigureMockMethods(services);
        }


    }
}
