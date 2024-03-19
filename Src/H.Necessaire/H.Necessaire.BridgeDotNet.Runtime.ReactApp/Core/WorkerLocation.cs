using Bridge;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    [Name("WorkerLocation")]
    public class WorkerLocation
    {
        [External]
        [Name("hash")]
        public static extern string Hash { get; }

        [External]
        [Name("host")]
        public static extern string Host { get; }

        [External]
        [Name("hostname")]
        public static extern string Hostname { get; }

        [External]
        [Name("href")]
        public static extern string Href { get; }

        [External]
        [Name("origin")]
        public static extern string Origin { get; }

        [External]
        [Name("pathname")]
        public static extern string PathName { get; }

        [External]
        [Name("port")]
        public static extern string Port { get; }

        [External]
        [Name("search")]
        public static extern string Search { get; }

        [External]
        [Name("toString")]
        public static extern string ToString();
    }
}