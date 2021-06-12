namespace H.Necessaire
{
    public static class IoC
    {
        public static ImADependencyRegistry NewDependencyRegistry() => new DependencyRegistry();
    }
}
