using H.Necessaire;
using System.Threading;

namespace System
{
    public class Lazy<T>
    {
        #region Construct
        readonly Func<T> valueFactory;
        readonly LazyThreadSafetyMode threadSafetyMode = LazyThreadSafetyMode.PublicationOnly;
        T value;
        public Lazy(Func<T> valueFactory, LazyThreadSafetyMode mode)
        {
            this.valueFactory = valueFactory;
            this.threadSafetyMode = mode;
        }

        public Lazy(Func<T> valueFactory) : this(valueFactory, LazyThreadSafetyMode.PublicationOnly) { }

        public Lazy(T value) : this(() => value, LazyThreadSafetyMode.PublicationOnly) { }
        #endregion

        public bool IsValueCreated { get; private set; } = false;

        public T Value
        {
            get
            {
                ConstructValueIdNecessary();
                return GetValue();
            }
        }

        public override string ToString()
        {
            return IsValueCreated ? Value?.ToString() : $"Value not yet created";
        }

        private void ConstructValueIdNecessary()
        {
            if (IsValueCreated)
                return;

            using (new ScopedRunner(onStart: () => { }, onStop: () => IsValueCreated = true))
            {
                T newValue = valueFactory();

                value = IsValueCreated ? value : newValue;
            }
        }

        private T GetValue()
        {
            if (!IsValueCreated)
                throw new InvalidOperationException("Value not yet constructed. Must call ConstructValueIdNecessary() before this");

            return value;
        }
    }
}
