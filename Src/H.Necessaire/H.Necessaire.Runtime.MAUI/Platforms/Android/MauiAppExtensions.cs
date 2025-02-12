using Android.Content.Res;
using H.Necessaire.Runtime.MAUI.Components;
using H.Necessaire.Runtime.MAUI.Extensions;
using Microsoft.Maui.Platform;

namespace H.Necessaire.Runtime.MAUI.Platforms.Android
{
    public static class MauiAppExtensions
    {
        public static MauiApp InitializeHNecessaire(this MauiApp mauiApp)
        {
            // Remove Editor control underline
            Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
            {
                h.PlatformView.BackgroundTintList = ColorStateList.ValueOf(HUiToolkit.Current.Branding.BackgroundColorTranslucent.ToMaui().ToPlatform());
            });

            return mauiApp;
        }
    }
}
