using H.Necessaire.Runtime.Integration.AspNetCore.Middlewares;
using H.Necessaire.Runtime.Integration.DotNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace H.Necessaire.Runtime.Integration.AspNetCore
{
    public static class WebAppWireupExtensions
    {
        public static THostApplicationBuilder WithHNecessaire<THostApplicationBuilder>(this THostApplicationBuilder hostApplicationBuilder, ImADependencyRegistry dependencyRegistry)
            where THostApplicationBuilder : IHostApplicationBuilder
        {
            hostApplicationBuilder
                .Services
                .AddHttpContextAccessor()
                .AddAntiforgery()
                .WithHNecessaireAspNetRuntime(dependencyRegistry, hostApplicationBuilder.Configuration)
                ;

            hostApplicationBuilder
                .Logging
                .ClearProviders()
                .AddHNecessaireLogging(dependencyRegistry)
                ;

            return hostApplicationBuilder;
        }

        public static TApplicationBuilder ConfigureHNecessaireAspNetRuntime<TApplicationBuilder>(this TApplicationBuilder applicationBuilder, IHostEnvironment hostEnvironment) where TApplicationBuilder : IApplicationBuilder
        {
            applicationBuilder.Use((ctx, next) => { ctx.Request.EnableBuffering(); return next(); });
            applicationBuilder.UseMiddleware<ExceptionHandlerMiddleware>();
            if (!hostEnvironment.IsEnvironment("Local"))
                applicationBuilder.UseHttpsRedirection();
            applicationBuilder.UseRouting();
            applicationBuilder.UseAntiforgery();
            applicationBuilder.UseCors();
            applicationBuilder.UseEndpoints(x => x.MapControllers());

            return applicationBuilder;
        }
    }
}
