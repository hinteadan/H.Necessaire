using System;
using System.Collections.Generic;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class UiNavigationParams
    {
        public static readonly UiNavigationParams None = new UiNavigationParams(null);

        private readonly Dictionary<string, object> parameters;

        public UiNavigationParams(Dictionary<string, object> parameters)
        {
            this.parameters = parameters ?? new Dictionary<string, object>();
        }

        public UiNavigationParams(object parameter)
            : this(new Dictionary<string, object>() { { string.Empty, parameter } })
        { }

        public T GetValueFor<T>(string parameter, Action orFail = null)
        {
            if (!parameters.ContainsKey(parameter))
            {
                orFail?.Invoke();
                return default(T);
            }

            object value = parameters[parameter];

            if (!(value is T))
            {
                orFail?.Invoke();
                return default(T);
            }

            return (T)value;
        }

        public T GetValue<T>(Action orFail = null) => GetValueFor<T>(string.Empty, orFail);
    }
}
