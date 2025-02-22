using H.Necessaire.CLI.Commands;
using H.Necessaire.Models.Branding;
using H.Necessaire.Runtime.UI.CLI.BLL;

namespace H.Necessaire.Runtime.UI.CLI
{
    [ID("maui-branding")]
    class MauiBrandingCommand : CommandBase
    {
        static readonly string[] usageSyntaxes = [
            "maui-branding colors app=AppFolderNameOrCsproj color-palette-class=ClassNameHoldingTheColorPallete color-palette-member=NameOfPropFieldOrMethodHoldingColorPalette [color-palette-assembly=AssemblyName]",
            "maui-branding icon app=AppFolderNameOrCsproj svg-icon-file=path/to/myappicon.svg [app-icon-base-size=100,100] [splash-icon-base-size=128,128]"
        ];
        protected override string[] GetUsageSyntaxes() => usageSyntaxes;
        public override Task<OperationResult> Run() => RunSubCommand();

        [ID("colors")]
        class ColorsSubCommand : SubCommandBase
        {
            #region Construct
            ColorPaletteFinder colorPaletteFinder;
            AppColorConfigProcessor appColorConfigProcessor;
            public override void ReferDependencies(ImADependencyProvider dependencyProvider)
            {
                base.ReferDependencies(dependencyProvider);
                colorPaletteFinder = dependencyProvider.Get<ColorPaletteFinder>();
                appColorConfigProcessor = dependencyProvider.Get<AppColorConfigProcessor>();
            }
            #endregion

            public override async Task<OperationResult> Run(params Note[] args)
            {
                OperationResult<ColorPalette> colorPaletteResult = colorPaletteFinder.Find(
                    args.Get("color-palette-class", ignoreCase: true),
                    args.Get("color-palette-member", ignoreCase: true),
                    args.Get("color-palette-assembly", ignoreCase: true)
                );

                if (!colorPaletteResult.IsSuccessful)
                    return colorPaletteResult;

                ColorPalette colorPalette = colorPaletteResult.Payload;

                string appName = args.Get("app", ignoreCase: true);
                if (appName.IsEmpty())
                    return OperationResult.Fail("app argument must be provided");

                string csProj
                    = appName.EndsWith(".csproj", StringComparison.InvariantCultureIgnoreCase)
                    ? appName
                    : $"{appName}.csproj"
                    ;

                OperationResult<DirectoryInfo> appRootFolderResult = new RepoFinder(csProj).Find();
                if (!appRootFolderResult.IsSuccessful)
                    return appRootFolderResult;

                DirectoryInfo appRootFolder = appRootFolderResult.Payload;

                OperationResult processResult = await appColorConfigProcessor.Process(appName, appRootFolder, colorPalette);

                return processResult;
            }
        }

        [ID("icon")]
        class IconSubCommand : SubCommandBase
        {
            #region Construct
            AppIconConfigProcessor appIconConfigProcessor;
            public override void ReferDependencies(ImADependencyProvider dependencyProvider)
            {
                base.ReferDependencies(dependencyProvider);
                appIconConfigProcessor = dependencyProvider.Get<AppIconConfigProcessor>();
            }
            #endregion

            public override async Task<OperationResult> Run(params Note[] args)
            {
                string svgIconFilePath = args.Get("svg-icon-file", ignoreCase: true);
                if (svgIconFilePath.IsEmpty())
                    return OperationResult.Fail("svg-icon-file argument must be provided");

                FileInfo svgIconFile = new FileInfo(svgIconFilePath);
                if (!svgIconFile.Exists)
                    return OperationResult.Fail($"svg-icon-file={svgIconFilePath} doesn't exist");

                string appName = args.Get("app", ignoreCase: true);
                if (appName.IsEmpty())
                    return OperationResult.Fail("app argument must be provided");

                string csProj
                    = appName.EndsWith(".csproj", StringComparison.InvariantCultureIgnoreCase)
                    ? appName
                    : $"{appName}.csproj"
                    ;

                OperationResult<DirectoryInfo> appRootFolderResult = new RepoFinder(csProj).Find();
                if (!appRootFolderResult.IsSuccessful)
                    return appRootFolderResult;

                DirectoryInfo appRootFolder = appRootFolderResult.Payload;

                OperationResult processResult = await appIconConfigProcessor.Process(
                    appName, 
                    appRootFolder, 
                    svgIconFile,
                    args.Get("app-icon-base-size", ignoreCase: true),
                    args.Get("splash-icon-base-size", ignoreCase: true)
                );

                return processResult;
            }
        }
    }
}
