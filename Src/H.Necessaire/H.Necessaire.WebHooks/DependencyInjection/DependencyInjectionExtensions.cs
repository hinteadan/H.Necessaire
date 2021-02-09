using H.Necessaire.WebHooks.Implementations;
using H.Necessaire.WebHooks.Implementations.SQLServer;
using System;

namespace H.Necessaire.WebHooks
{
    public static class DependencyInjectionExtensions
    {
        public static void AddHNecessaireWebHooksWithSqlServerImplementations(
            Action<Type, Type> singletonRegistrarByInterfaceType,
            Action<Type, Func<IServiceProvider, object>> singletonRegistrarByFactory,
            Func<IServiceProvider, string> sqlConnectionStringProvider,
            Func<IServiceProvider, IWebHookProcessor[]> webHookProcessorsFactory
        )
        {
            singletonRegistrarByFactory?.Invoke(typeof(IWebHookProcessor[]), webHookProcessorsFactory);
            singletonRegistrarByFactory?.Invoke(typeof(IWebHookRequestStorage), x => new SqlServerWebHookRequestStore(sqlConnectionStringProvider(x)));
            singletonRegistrarByInterfaceType?.Invoke(typeof(IWebHookService), typeof(DefaultWebHookService));
        }
    }
}
