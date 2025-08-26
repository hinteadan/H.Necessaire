using Microsoft.JSInterop;

namespace H.Necessaire.Runtime.UI.Razor
{
    public class HJs : IAsyncDisposable, IDebug
    {
        readonly IJSRuntime js;
        readonly Lazy<Task<IJSObjectReference>> hJsModuleTask;
        public HJs(IJSRuntime js)
        {
            this.js = js;
            hJsModuleTask = new(async () => await js.InvokeAsync<IJSObjectReference>("import", "./_content/H.Necessaire.Runtime.UI.Razor/h.js"));
        }

        public async Task Debug()
        {
            IJSObjectReference hjs = await hJsModuleTask.Value;

            await hjs.InvokeVoidAsync("Debug", "string from C#");
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
}
