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
        private readonly Func<Type, object> typedFactory;
        private object instance = null;

        public bool IsAlwaysNew { get; } = false;

        public InstanceFactory(ImADependencyProvider dependencyProvider, Func<object> factory, bool isAlwaysNew = false)
        {
            this.dependencyRegistry = dependencyProvider as DependencyRegistry;
            this.dependencyProvider = dependencyProvider;
            this.factory = factory;
            this.typedFactory = null;
            this.IsAlwaysNew = isAlwaysNew;
        }

        public InstanceFactory(ImADependencyProvider dependencyProvider, Func<Type, object> typedFactory, bool isAlwaysNew = false)
        {
            this.dependencyRegistry = dependencyProvider as DependencyRegistry;
            this.dependencyProvider = dependencyProvider;
            this.factory = null;
            this.typedFactory = typedFactory;
            this.IsAlwaysNew = isAlwaysNew;
        }

        public InstanceFactory(ImADependencyProvider dependencyProvider, object instance) : this(dependencyProvider, () => instance, isAlwaysNew: false) { }

        public object GetInstance(Type typeToEnsure = null)
        {
            EnsureInstance(typeToEnsure);
            return instance;
        }

        private void EnsureInstance(Type typeToEnsure = null)
        {
            lock (locker)
            {
                if (instance != null && !IsAlwaysNew)
                    return;

                instance = typeToEnsure is null ? factory?.Invoke() : (typedFactory?.Invoke(typeToEnsure) ?? factory?.Invoke());
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
