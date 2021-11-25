using H.Necessaire.BridgeDotNet.Runtime.ReactApp;
using H.Necessaire.Models.Branding;
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
            await Task.Delay(1000);

            await Debug();

            await AppLogger.LogTrace("DONE ReactAppSample App Initialize");
        }

        private static async Task Debug()
        {
            await Task.Delay(500);
        }
    }
}
