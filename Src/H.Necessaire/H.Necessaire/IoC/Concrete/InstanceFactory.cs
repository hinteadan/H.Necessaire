using System;
using System.Collections.Concurrent;

namespace H.Necessaire
{
    class InstanceFactory
    {
        static readonly object locker = new object();

        private readonly DependencyRegistry dependencyRegistry;
        private readonly ImADependencyProvider dependencyProvider;
        private readonly Func<object> factory;
        private object instance = null;

        public bool IsAlwaysNew { get; } = false;

        public InstanceFactory(ImADependencyProvider dependencyProvider, Func<object> factory, bool isAlwaysNew = false)
        {
            this.dependencyRegistry = dependencyProvider as DependencyRegistry;
            this.dependencyProvider = dependencyProvider;
            this.factory = factory;
            this.IsAlwaysNew = isAlwaysNew;
        }

        public InstanceFactory(ImADependencyProvider dependencyProvider, object instance) : this(dependencyProvider, () => instance, isAlwaysNew: false) { }

        public object GetInstance()
        {
            EnsureInstance();
            return instance;
        }

        private void EnsureInstance()
        {
            lock (locker)
            {
                if (instance != null && !IsAlwaysNew)
                    return;

                instance = factory?.Invoke();
                if (instance is ImADependency dependency)
                {
                    Type type = dependency.GetType();

                    bool isReferAlreadyCalled = false;
                    if (!IsAlwaysNew) isReferAlreadyCalled = dependencyRegistry?.IsReferDepsAlreadyCalledFor(type) ?? false;

                    if (!isReferAlreadyCalled)
                        dependency.ReferDependencies(dependencyProvider);

                    if(!IsAlwaysNew)
                        dependencyRegistry?.PinReferDepsCallFor(type);
                }
            }
        }
    }
}
