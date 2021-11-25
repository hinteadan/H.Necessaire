using System;
using System.Diagnostics;

namespace H.Necessaire
{
    class InstanceFactory
    {
        static object locker = new object();

        private readonly ImADependencyProvider dependencyProvider;
        private readonly Func<object> factory;
        private object instance;

        public bool IsAlwaysNew { get; } = false;

        public InstanceFactory(ImADependencyProvider dependencyProvider, Func<object> factory, bool isAlwaysNew = false)
        {
            this.dependencyProvider = dependencyProvider;
            this.factory = factory;
            this.IsAlwaysNew = isAlwaysNew;
        }

        public InstanceFactory(ImADependencyProvider dependencyProvider, object instance)
            : this(dependencyProvider, () => instance, isAlwaysNew: false)
        {
            this.instance = instance;
            this.factory = () => this.instance;
        }

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

                instance = factory();
                if (instance is ImADependency)
                {
                    (instance as ImADependency).ReferDependencies(dependencyProvider);
                }
            }
        }
    }
}
