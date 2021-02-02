using H.Necessaire.WebSockets.Concrete;
using System;

namespace H.Necessaire.WebSockets
{
    public static class DependencyInjectionExtensions
    {
        public static void AddHNecessaireWebSockets(
            Action<Type> singletonRegistrarByType,
            Action<Type, Type> singletonRegistrarByInterfaceType,
            Action<Type, Func<IServiceProvider, object>> singletonRegistrarByFactory,
            Func<IServiceProvider, string> webSocketServerBaseUrlProvider
        )
        {
            singletonRegistrarByType(typeof(WebSocketClientNotificationQueue));

            singletonRegistrarByFactory(typeof(IWebSocketClientNotifier), x => x.GetService(typeof(WebSocketClientNotificationQueue)));

            singletonRegistrarByFactory(
                typeof(IWebSocketServerService),
                x => new WebSocketServerService(
                    webSocketServerBaseUrlProvider(x),
                    (WebSocketClientNotificationQueue)x.GetService(typeof(WebSocketClientNotificationQueue))
                )
            );
        }
    }
}
