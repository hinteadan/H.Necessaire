namespace H.Necessaire.Operations.Caching.Concrete
{
    internal class CacherFactory : ImACacherFactory, ImADependency
    {
        ImADependencyProvider dependencyProvider;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            this.dependencyProvider = dependencyProvider;
        }

        public ImACacher<T> BuildCacher<T>(string cacherID = "InMemory")
        {
            ImACacher<T> cacher =
                cacherID.IsEmpty()
                ? dependencyProvider?.Get<ImACacher<T>>()
                : dependencyProvider?.Build<ImACacher<T>>(cacherID, dependencyProvider?.Get<ImACacher<T>>())
                ;

            if (cacher == null)
                return null;

            return cacher;
        }
    }
}
