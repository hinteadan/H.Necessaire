using H.Necessaire.Runtime.Integration.AspNetCore.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace H.Necessaire.Runtime.Integration.AspNetCore
{
    internal static class WebAppWireupExtensions
    {
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
