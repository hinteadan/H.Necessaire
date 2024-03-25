namespace H.Necessaire.CLI.Commands.HDoc.BLL
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<HDocCsProjParser>(() => new HDocCsProjParser())
                .Register<HDocCsFileParser>(() => new HDocCsFileParser())
                .Register<HDocTypeProcessor>(() => new HDocTypeProcessor())

                .Register<HDocConstructorProcessor>(() => new HDocConstructorProcessor())
                .Register<HDocMethodProcessor>(() => new HDocMethodProcessor())
                .Register<HDocPropertyProcessor>(() => new HDocPropertyProcessor())
                .Register<HDocParameterProcessor>(() => new HDocParameterProcessor())
                .Register<HDocFieldProcessor>(() => new HDocFieldProcessor())

                .Register<Reporting.DependencyGroup>(() => new Reporting.DependencyGroup())
                ;
        }
    }
}
