using H.Necessaire;
using H.Necessaire.AspNetCoreWebAppSample;
using H.Necessaire.Runtime.Integration.NetCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Program
{
    public static readonly App App = new App(new AppWireup().WithEverything());

    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    static IWebHostBuilder CreateWebHostBuilder(string[] args)
    {
        return
            WebHost

            .CreateDefaultBuilder(args)

            .ConfigureServices(x =>
            {
                x.AddHNecessaireDependenciesToNetCore(App.Wireup.DependencyRegistry);
            })

            .ConfigureLogging((context, logging) =>
            {
                logging
                    .ClearProviders()
                    .AddHNecessaireLogging(App.Wireup.DependencyRegistry)
                    ;
            })

            .UseStartup<Startup>();
    }
}
