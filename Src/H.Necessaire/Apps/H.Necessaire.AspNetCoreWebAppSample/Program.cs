using H.Necessaire.AspNetCoreWebAppSample;
using H.Necessaire.AspNetCoreWebAppSample.Components;
using H.Necessaire.Runtime.Integration.AspNetCore;
using H.Necessaire.Runtime.UI.Razor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        var builder
            = WebApplication
            .CreateBuilder(args)
            .WithHNecessaire(HAspNetCoreSampleApp.Instance.DependencyRegistry.WithDefaultHAppConfig())
            ;

        // Add services to the container.
        builder
            .Services
            .WithHRazorRuntime<HRazorApp>(HAspNetCoreSampleApp.Instance, deps: reg => reg.Register<DependencyGroup>(() => new DependencyGroup()))
            .AddRazorComponents()
            .AddInteractiveServerComponents()
            ;

        var app = builder.Build().BindToHNecessaireAspNetRuntime(HAspNetCoreSampleApp.Instance.DependencyRegistry);

        app.ConfigureHNecessaireAspNetRuntime(app.Environment);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            //Do some dev related stuff
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            ;

        app.Run();
    }
}
