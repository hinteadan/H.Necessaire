using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire
{
    class DependencyRegistry : ImADependencyRegistry
    {
        #region Construct
        readonly ConcurrentDictionary<Type, InstanceFactory> dependencyDictionary = new ConcurrentDictionary<Type, InstanceFactory>();
        #endregion

        public ImADependencyRegistry Register(Type type, Func<object> factory)
        {
            object instance = null;
            if (typeof(ImADependencyGroup).IsAssignableFrom(type))
            {
                instance = factory();
                ((ImADependencyGroup)instance).RegisterDependencies(this);
            }

            InstanceFactory instanceFactory = new InstanceFactory(this, instance == null ? factory : () => instance, isAlwaysNew: false);
            dependencyDictionary.AddOrUpdate(type, instanceFactory, (x, y) => instanceFactory);
            return this;
        }
        public ImADependencyRegistry Register<T>(Func<object> factory) => Register(typeof(T), factory);

        public ImADependencyRegistry RegisterAlwaysNew(Type type, Func<object> factory)
        {
            if (typeof(ImADependencyGroup).IsAssignableFrom(type))
            {
                throw new InvalidOperationException("Dependecy groups must be registered using Register()");
            }

            InstanceFactory instanceFactory = new InstanceFactory(this, factory, isAlwaysNew: true);
            dependencyDictionary.AddOrUpdate(type, instanceFactory, (x, y) => instanceFactory);
            return this;
        }
        public ImADependencyRegistry RegisterAlwaysNew<T>(Func<object> factory) => RegisterAlwaysNew(typeof(T), factory);

        public ImADependencyRegistry Unregister(Type type)
        {
            InstanceFactory removedFactory;
            dependencyDictionary.TryRemove(type, out removedFactory);
            return this;
        }
        public ImADependencyRegistry Unregister<T>() => Unregister(typeof(T));

        public object Get(Type type)
        {
            if (!dependencyDictionary.ContainsKey(type))
                return null;

            return dependencyDictionary[type].GetInstance();
        }
        public T Get<T>() => (T)Get(typeof(T));

        public IEnumerable<KeyValuePair<Type, Func<object>>> GetAllOneTimeTypes()
        {
            return
                dependencyDictionary
                .Where(x => !x.Value.IsAlwaysNew)
                .Select(x => new KeyValuePair<Type, Func<object>>(x.Key, x.Value.GetInstance));
        }

        public IEnumerable<KeyValuePair<Type, Func<object>>> GetAllAlwaysNewTypes()
        {
            return
                dependencyDictionary
                .Where(x => x.Value.IsAlwaysNew)
                .Select(x => new KeyValuePair<Type, Func<object>>(x.Key, x.Value.GetInstance));
        }
    }
}