using H.Necessaire.Runtime.Integration.DotNet.Concrete;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System;

namespace H.Necessaire.Runtime.Integration.DotNet
{
    public static class NetCoreLoggingExtensions
    {
        public static LogEntryLevel ToLogEntryLevel(this LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace: return LogEntryLevel.Trace;
                case LogLevel.Debug: return LogEntryLevel.Debug;
                case LogLevel.Information: return LogEntryLevel.Info;
                case LogLevel.Warning: return LogEntryLevel.Warn;
                case LogLevel.Error: return LogEntryLevel.Error;
                case LogLevel.Critical: return LogEntryLevel.Critical;
                case LogLevel.None:
                default: return LogEntryLevel.None;
            }
        }

        public static ILoggingBuilder AddHNecessaireLogging(this ILoggingBuilder builder, ImADependencyProvider dependencyProvider)
        {
            builder.AddConfiguration();

            builder.Services.AddSingleton<ILoggerProvider, NetCoreLoggerProvider>(_ => dependencyProvider.Get<NetCoreLoggerProvider>());

            LoggerProviderOptions.RegisterProviderOptions<RuntimeConfig, NetCoreLoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddHNecessaireLogging(this ILoggingBuilder builder, Func<IServiceProvider, ImADependencyProvider> dependencyProvider)
        {
            builder.AddConfiguration();

            builder.Services.AddSingleton<ILoggerProvider, NetCoreLoggerProvider>(sp => dependencyProvider.Invoke(sp).Get<NetCoreLoggerProvider>());

            LoggerProviderOptions.RegisterProviderOptions<RuntimeConfig, NetCoreLoggerProvider>(builder.Services);

            return builder;
        }
    }
}
