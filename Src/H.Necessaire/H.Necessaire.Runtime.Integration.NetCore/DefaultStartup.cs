using H.Necessaire.Runtime.Integration.NetCore.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace H.Necessaire.Runtime.Integration.NetCore
{
    public class DefaultStartup
    {

        #region Construct
        readonly ILogger<DefaultStartup> logger;
        readonly ImADependencyRegistry dependencyRegistry;
        public DefaultStartup(
            ImADependencyRegistry dependencyRegistry,
            ILogger<DefaultStartup> logger
        )
        {
            this.dependencyRegistry = dependencyRegistry;
            this.logger = logger;
        }
        #endregion

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            using (new TimeMeasurement(x => logger.LogInformation($"Done wiring-up H.Necessaire in {x}")))
                services.AddNetCoreDependenciesToHNecessaire(dependencyRegistry);

            using (new TimeMeasurement(x => logger.LogInformation($"Done adding CORS in {x}")))
                services.AddCors(opts => opts.AddDefaultPolicy(bld => bld.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

            using (new TimeMeasurement(x => logger.LogInformation($"Done adding routing in {x}")))
                services.AddRouting();

            using (new TimeMeasurement(x => logger.LogInformation($"Done adding controllers in {x}")))
                ConfigureControllers(services);

            using (new TimeMeasurement(x => logger.LogInformation($"Done adding extras in {x}")))
                ConfigureExtraServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            using (new TimeMeasurement(x => logger.LogInformation($"Done configuring Web App Host in {x}")))
                ConfigureWebAppHost(app, env);
        }

        protected void ConfigureWebAppHost(IApplicationBuilder app, IHostEnvironment env)
        {
            using (new TimeMeasurement(x => logger.LogInformation($"Done calling HttpContext.Request Enable Buffering in {x}")))
                app.Use((ctx, next) => { ctx.Request.EnableBuffering(); return next(); });

            using (new TimeMeasurement(x => logger.LogInformation($"Done calling Use ExceptionHandlerMiddleware in {x}")))
                app.UseMiddleware<ExceptionHandlerMiddleware>();

            if (!env.IsEnvironment("Local"))
            {
                using (new TimeMeasurement(x => logger.LogInformation($"Done calling UseHttpsRedirection in {x}")))
                    app.UseHttpsRedirection();
            }

            using (new TimeMeasurement(x => logger.LogInformation($"Done calling UseRouting in {x}")))
                ConfigureAppRouting(app, env);

            using (new TimeMeasurement(x => logger.LogInformation($"Done calling UseCors in {x}")))
                app.UseCors();

            using (new TimeMeasurement(x => logger.LogInformation($"Done registering API controllers in {x}")))
                ConfigureAppEndpoints(app, env);

            using (new TimeMeasurement(x => logger.LogInformation($"Done calling UseDefaultFiles in {x}")))
                app
                    .UseDefaultFiles(new DefaultFilesOptions
                    {
                        FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Content")),
                        RequestPath = string.Empty,
                    });

            using (new TimeMeasurement(x => logger.LogInformation($"Done calling UseStaticFiles in {x}")))
                app
                    .UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Content")),
                        RequestPath = string.Empty,
                    })
                    .UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Content")),
                        RequestPath = "/css",
                    })
                    .UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Content")),
                        RequestPath = "/webfonts",
                    })
                    ;

            using (new TimeMeasurement(x => logger.LogInformation($"Done configuring App Extras in {x}")))
                ConfigureAppExtras(app, env);
        }

        protected virtual IServiceCollection ConfigureControllers(IServiceCollection services)
        {
            return services;
        }

        protected virtual IServiceCollection ConfigureExtraServices(IServiceCollection services)
        {
            return services;
        }

        protected virtual IApplicationBuilder ConfigureAppRouting(IApplicationBuilder app, IHostEnvironment env)
        {
            return app;
        }

        protected virtual IApplicationBuilder ConfigureAppEndpoints(IApplicationBuilder app, IHostEnvironment env)
        {
            return app;
        }

        protected virtual IApplicationBuilder ConfigureAppExtras(IApplicationBuilder app, IHostEnvironment env)
        {
            return app;
        }
    }
}
