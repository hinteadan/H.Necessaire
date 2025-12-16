using Microsoft.Extensions.Logging;
using System;

namespace H.Necessaire.Runtime.Integration.DotNet.Concrete
{
    internal class NetCoreLoggerProvider : ILoggerProvider, ImADependency
    {
        Func<string, ImALogger> buildLogger;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            buildLogger = x => dependencyProvider?.GetLogger(x, "H.Necessaire.Runtime.Integration.NetCore");
        }

        public ILogger CreateLogger(string categoryName) => new NetCoreLogger(buildLogger(categoryName));

        public void Dispose() { }
    }
}
