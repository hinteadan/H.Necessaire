using H.Necessaire.Runtime.Integration.NetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace H.Necessaire.AspNetCoreWebAppSample
{
    public class Startup : DefaultStartup
    {
        public Startup(ImADependencyRegistry dependencyRegistry, ILogger<DefaultStartup> logger) : base(dependencyRegistry, logger)
        {
        }

        protected override IServiceCollection ConfigureControllers(IServiceCollection services)
        {
            base.ConfigureControllers(services);

            services
                .AddControllers()
                .AddApplicationPart(typeof(H.Necessaire.Runtime.Integration.NetCore.Controllers.PingController).Assembly)
                .AddControllersAsServices()
                ;

            return services;
        }

        protected override IApplicationBuilder ConfigureAppRouting(IApplicationBuilder app, IHostEnvironment env)
        {
            return
                base
                .ConfigureAppRouting(app, env)
                .UseRouting()
                ;
        }

        protected override IApplicationBuilder ConfigureAppEndpoints(IApplicationBuilder app, IHostEnvironment env)
        {
            return
                base
                .ConfigureAppEndpoints(app, env)
                .UseEndpoints(x =>
                {
                    x.MapControllers();
                })
                ;
        }
    }
}
