using H.Necessaire.BridgeDotNet.Runtime.ReactApp;
using H.Necessaire.Models.Branding;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.ReactAppSample
{
    public class App : AppBase
    {
        public static void Main()
        {
            Initialize(new AppWireup(), x => new AppNavigationRegistry(x), Run, branding: BrandingStyle.Default);
            MainAsync();
        }

        static async Task Run()
        {
            await Task.Delay(500);
            Console.WriteLine($"DONE OWN App Initialize");
        }
    }
}
