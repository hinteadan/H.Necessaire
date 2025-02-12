using Android.App;
using Android.Runtime;
using H.Necessaire.Runtime.MAUI.Platforms.Android;

namespace H.Necessaire.Runtime.MAUIAppSample
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp().InitializeHNecessaire();
    }
}
