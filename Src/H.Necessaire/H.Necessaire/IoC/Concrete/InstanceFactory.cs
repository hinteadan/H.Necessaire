using System;

namespace H.Necessaire
{
    class InstanceFactory
    {
        static object locker = new object();

        private readonly ImADependencyProvider dependencyProvider;
        private readonly Func<object> factory;
        private Lazy<object> lazyInstance = null;

        public bool IsAlwaysNew { get; } = false;

        public InstanceFactory(ImADependencyProvider dependencyProvider, Func<object> factory, bool isAlwaysNew = false)
        {
            this.dependencyProvider = dependencyProvider;
            this.factory = factory;
            this.IsAlwaysNew = isAlwaysNew;
        }

        public InstanceFactory(ImADependencyProvider dependencyProvider, object instance) : this(dependencyProvider, () => instance, isAlwaysNew: false) { }

        public object GetInstance()
        {
            EnsureInstance();
            return lazyInstance.Value;
        }

        private void EnsureInstance()
        {
            lock (locker)
            {
                if (lazyInstance != null && !IsAlwaysNew)
                    return;

                lazyInstance = new Lazy<object>(factory);
                if (lazyInstance.Value is ImADependency)
                {
                    (lazyInstance.Value as ImADependency).ReferDependencies(dependencyProvider);
                }
            }
        }
    }
}
