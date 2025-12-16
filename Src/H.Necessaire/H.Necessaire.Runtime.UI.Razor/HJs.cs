using Microsoft.JSInterop;

namespace H.Necessaire.Runtime.UI.Razor
{
    public class HJs : IAsyncDisposable, IDebug
    {
        readonly IJSRuntime js;
        readonly Lazy<Task<IJSObjectReference>> hJsModuleTask;
        readonly HUiToolkit uiToolikit;
        public HJs(IJSRuntime js, HUiToolkit uiToolikit)
        {
            this.js = js;
            this.uiToolikit = uiToolikit;
            hJsModuleTask = new(async () => await js.InvokeAsync<IJSObjectReference>("import", "./_content/H.Necessaire.Runtime.UI.Razor/h.js"));
        }

        public async Task Debug()
        {
            await ApplyBranding();

            IJSObjectReference hjs = await hJsModuleTask.Value;

            await hjs.InvokeVoidAsync("Debug");
        }

        public async Task ApplyBranding()
        {
            IJSObjectReference hjs = await hJsModuleTask.Value;

            await hjs.InvokeVoidAsync("ApplyBranding", new
            {
                BackgroundColor = uiToolikit.Branding.BackgroundColorTranslucent.ToCssRGBA(),
                HighlightColor = uiToolikit.Branding.SecondaryColorTranslucent.ToCssRGBA(),
                PrimaryColor = uiToolikit.Branding.PrimaryColor.ToCssRGBA(),
                PrimaryColorFaded = uiToolikit.Branding.PrimaryColorFaded.ToCssRGBA(),
                PrimaryColorTranslucent = uiToolikit.Branding.PrimaryColorTranslucent.ToCssRGBA(),
                FontFamily = uiToolikit.Branding.Typography.FontFamily,
                FontSize = uiToolikit.Branding.Typography.FontSize.PointsCss,
                SizingUnit = uiToolikit.SizingUnit,
            });
        }

        public async Task<HScrollInfo> GetScrollPosition(string elementID)
        {
            IJSObjectReference hjs = await hJsModuleTask.Value;

            return await hjs.InvokeAsync<HScrollInfo>("GetScrollPosition", elementID);
        }

        public async Task<ConsumerIdentity> GetConsumerInfo(Guid newConsumerID)
        {
            IJSObjectReference hjs = await hJsModuleTask.Value;
            return await hjs.InvokeAsync<ConsumerIdentity>("GetConsumerInfo", newConsumerID);
        }

        public async Task SetSessionValue(string key, string value)
        {
            IJSObjectReference hjs = await hJsModuleTask.Value;
            await hjs.InvokeVoidAsync("SetSessionValue", key, value);
        }

        public async Task<string> GetSessionValue(string key)
        {
            IJSObjectReference hjs = await hJsModuleTask.Value;
            return await hjs.InvokeAsync<string>("GetSessionValue", key);
        }

        public async Task ZapSessionValue(string key)
        {
            IJSObjectReference hjs = await hJsModuleTask.Value;
            await hjs.InvokeVoidAsync("ZapSessionValue", key);
        }

        public async Task SetLocalValue(string key, string value)
        {
            IJSObjectReference hjs = await hJsModuleTask.Value;
            await hjs.InvokeVoidAsync("SetLocalValue", key, value);
        }

        public async Task<string> GetLocalValue(string key)
        {
            IJSObjectReference hjs = await hJsModuleTask.Value;
            return await hjs.InvokeAsync<string>("GetLocalValue", key);
        }

        public async Task ZapLocalValue(string key)
        {
            IJSObjectReference hjs = await hJsModuleTask.Value;
            await hjs.InvokeVoidAsync("ZapLocalValue", key);
        }

        public async ValueTask DisposeAsync()
        {
            if (!hJsModuleTask.IsValueCreated)
                return;

            await HSafe.Run(async () =>
            {
                IJSObjectReference module = await hJsModuleTask.Value;
                await module.DisposeAsync();
            });
        }
    }

    public class HScrollInfo
    {
        public double ScrollTop { get; set; }
        public double ScrollLeft { get; set; }
        public double ScrollHeight { get; set; }
        public double ClientHeight { get; set; }

        public bool IsAtTheBottom(double? tolerance = null)
        {
            tolerance ??= ClientHeight / 6;
            tolerance = Math.Max(tolerance.Value, 200);
            return ScrollTop + ClientHeight >= ScrollHeight - tolerance.Value;
        }
    }
}
