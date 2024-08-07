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

        public static T WithHNecessaireWebSockets<T>(this T dependencyRegistry, Func<string> webSocketServerBaseUrlProvider) where T : ImADependencyRegistry
        {
            dependencyRegistry
                .Register<WebSocketClientNotificationQueue>(() => new WebSocketClientNotificationQueue())
                .Register<IWebSocketClientNotifier>(() => dependencyRegistry.Get<WebSocketClientNotificationQueue>())
                .Register<IWebSocketServerService>(() => new WebSocketServerService(webSocketServerBaseUrlProvider(), dependencyRegistry.Get<WebSocketClientNotificationQueue>()))
                ;

            return dependencyRegistry;
        }
    }
}
