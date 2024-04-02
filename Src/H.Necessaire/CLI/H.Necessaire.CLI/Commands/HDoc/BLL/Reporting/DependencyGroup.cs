namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<HTML.DependencyGroup>(() => new HTML.DependencyGroup())
                .Register<HDocumentationMarkdownReporter>(() => new HDocumentationMarkdownReporter())
                .Register<HDocumentationStaticWebSiteReporter>(() => new HDocumentationStaticWebSiteReporter())
                ;
        }
    }
}
