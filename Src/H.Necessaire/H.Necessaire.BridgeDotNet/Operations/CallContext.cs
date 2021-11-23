using System.Collections.Concurrent;

namespace H.Necessaire
{
    public static class CallContext
    {
        static ConcurrentDictionary<string, object> state = new ConcurrentDictionary<string, object>();

        public static void SetData(string name, object data) => state.AddOrUpdate(name, data, (a, b) => data);

        public static object GetData(string name) => state.ContainsKey(name) ? state[name] : null;
    }

    public static class CallContext<T>
    {
        static ConcurrentDictionary<string, T> state = new ConcurrentDictionary<string, T>();

        public static void SetData(string name, T data) => state.AddOrUpdate(name, data, (a, b) => data);

        public static T GetData(string name) => state.ContainsKey(name) ? state[name] : default(T);
    }
}
