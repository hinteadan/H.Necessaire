namespace H.Necessaire.Runtime.UI.CLI.BLL
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<FluentUiRepoFinder>(() => new FluentUiRepoFinder())
                .Register<FluentUiGlyphsParser>(() => new FluentUiGlyphsParser())
                .Register<FluentUiGlyphsExporter>(() => new FluentUiGlyphsExporter())
                .Register<HNecessaireMauiRepoFinder>(() => new HNecessaireMauiRepoFinder())
                .Register<ColorPaletteFinder>(() => new ColorPaletteFinder())
                .Register<AppColorConfigProcessor>(() => new AppColorConfigProcessor())
                .Register<AppIconConfigProcessor>(() => new AppIconConfigProcessor())
                ;
        }
    }
}
