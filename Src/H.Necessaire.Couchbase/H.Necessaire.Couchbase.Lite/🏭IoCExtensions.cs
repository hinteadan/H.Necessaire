namespace H.Necessaire.Couchbase.Lite
{
    public static class IoCExtensions
    {
        public static T WithHNecessaireCouchbaseLite<T>(this T depsRegistry) where T : ImADependencyRegistry
        {
            return depsRegistry;
        }
    }
}
