using System;

namespace H.Necessaire.Operations.Logging.Concrete
{
    internal class LoggerFactory : ImALoggerFactory, ImADependency
    {
        Func<InMemoryLogger> inMemoryLoggerFactory;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            inMemoryLoggerFactory = () => dependencyProvider.Get<InMemoryLogger>();
        }

        public ImALogger BuildLogger(string component, string application = "H.Necessaire")
        {
            return
                inMemoryLoggerFactory()
                .And(x =>
                {
                    x.Application = application;
                    x.Component = component;
                });
        }

        public ImALogger BuildLogger<T>(string application = "H.Necessaire")
        {
            return
                inMemoryLoggerFactory()
                .And(x =>
                {
                    x.Application = application;
                    x.Component = typeof(T).TypeName();
                });
        }
    }
}
