using Microsoft.JSInterop;

namespace H.Necessaire.Runtime.UI.Razor
{
    public static class HJsGlobals
    {
        static readonly AsyncEventRaiser<EventArgs> OnEscapeKeyPressedRaiser = new AsyncEventRaiser<EventArgs>(null);
        public static event AsyncEventHandler<EventArgs> OnEscapeKeyPressed { add => OnEscapeKeyPressedRaiser.AddHandler(value); remove => OnEscapeKeyPressedRaiser.ZapHandler(value); }

        static bool isHandlingEscapeKeyPressed = false;
        [JSInvokable("HJsGlobals.EscapeKeyPressed")]
        public static async Task EscapeKeyPressed()
        {
            if (isHandlingEscapeKeyPressed)
                return;

            using (new ScopedRunner(_ => isHandlingEscapeKeyPressed = true, _ => isHandlingEscapeKeyPressed = false))
            {
                await OnEscapeKeyPressedRaiser.Raise(EventArgs.Empty);
            }
        }
    }
}
