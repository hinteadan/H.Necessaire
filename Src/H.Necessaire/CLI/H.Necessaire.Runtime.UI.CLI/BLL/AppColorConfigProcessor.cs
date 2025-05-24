using H.Necessaire.Models.Branding;
using System.Text;

namespace H.Necessaire.Runtime.UI.CLI.BLL
{
    class AppColorConfigProcessor
    {
        public async Task<OperationResult> Process(string appName, DirectoryInfo appRootFolder, ColorPalette colorPalette)
        {
            string csProj
                = appName.EndsWith(".csproj", StringComparison.InvariantCultureIgnoreCase)
                ? appName
                : $"{appName}.csproj"
                ;

            FileInfo csProjFile = new FileInfo(Path.Combine(appRootFolder.FullName, csProj));
            FileInfo colorsXamlFile = new FileInfo(Path.Combine(appRootFolder.FullName, "Resources", "Styles", "Colors.xaml"));
            FileInfo androidColorsXmlFile = new FileInfo(Path.Combine(appRootFolder.FullName, "Platforms", "Android", "Resources", "values", "colors.xml"));
            FileInfo appIconBackgroundColorFile = new FileInfo(Path.Combine(appRootFolder.FullName, "Resources", "AppIcon", "appicon.svg"));


            ColorInfo primaryColor = colorPalette.Primary.Color;
            ColorInfo primaryDarkColor = colorPalette.Primary.Darker(2);
            ColorInfo primaryDarkTextColor = colorPalette.Primary.Darker(6);

            ColorInfo secondaryColor = colorPalette.PrimaryIsh(1).Color;
            ColorInfo secondaryDarkTextColor = colorPalette.PrimaryIsh(1).Darker(6);

            ColorInfo tertiaryColor = colorPalette.PrimaryIsh(2).Darker(1);

            await Task.WhenAll(
                ProcessAppCsProj(csProjFile, primaryColor),
                ProcessAppColorsXaml(colorsXamlFile, primaryColor, primaryDarkColor, primaryDarkTextColor, secondaryColor, secondaryDarkTextColor, tertiaryColor),
                ProcessAndroidColorsXml(androidColorsXmlFile, primaryColor, primaryDarkColor),
                ProcessAppIconBackgroundColor(appIconBackgroundColorFile, primaryColor)
            );

            return OperationResult.Win();
        }

        static async Task ProcessAppIconBackgroundColor(FileInfo appIconBackgroundColorFile, ColorInfo primaryColor)
        {
            string appIconBackgroundColorFileContent = await File.ReadAllTextAsync(appIconBackgroundColorFile.FullName);

            StringBuilder newContent = new StringBuilder();

            int hexLength = primaryColor.Hex.Length;

            int startFrom = 0;
            string startTag = "<rect x=\"0\" y=\"0\" width=\"456\" height=\"456\" fill=\"";
            int index = appIconBackgroundColorFileContent.IndexOf(startTag, startFrom) + startTag.Length;
            newContent
                .Append(appIconBackgroundColorFileContent.Substring(startFrom, index - startFrom))
                .Append(primaryColor.Hex)
                .And(_ => startFrom = index + hexLength);

            newContent
                .Append(appIconBackgroundColorFileContent.Substring(startFrom))
                ;

            await File.WriteAllTextAsync(appIconBackgroundColorFile.FullName, newContent.ToString());
        }

        static async Task ProcessAndroidColorsXml(FileInfo androidColorsXmlFile, ColorInfo primaryColor, ColorInfo primaryDarkColor)
        {
            string androidColorsXmlContent = await File.ReadAllTextAsync(androidColorsXmlFile.FullName);

            StringBuilder newContent = new StringBuilder();
            int hexLength = primaryColor.Hex.Length;

            int startFrom = 0;
            string startTag = "<color name=\"colorPrimary\">";
            int index = androidColorsXmlContent.IndexOf(startTag, startFrom) + startTag.Length;
            newContent
                .Append(androidColorsXmlContent.Substring(startFrom, index - startFrom))
                .Append(primaryColor.Hex)
                .And(_ => startFrom = index + hexLength);

            startTag = "<color name=\"colorPrimaryDark\">";
            index = androidColorsXmlContent.IndexOf(startTag, startFrom) + startTag.Length;
            newContent
                .Append(androidColorsXmlContent.Substring(startFrom, index - startFrom))
                .Append(primaryDarkColor.Hex)
                .And(_ => startFrom = index + hexLength)
                ;

            startTag = "<color name=\"colorAccent\">";
            index = androidColorsXmlContent.IndexOf(startTag, startFrom) + startTag.Length;
            newContent
                .Append(androidColorsXmlContent.Substring(startFrom, index - startFrom))
                .Append(primaryDarkColor.Hex)
                .And(_ => startFrom = index + hexLength)
                ;

            newContent
                .Append(androidColorsXmlContent.Substring(startFrom))
                ;

            await File.WriteAllTextAsync(androidColorsXmlFile.FullName, newContent.ToString());
        }

        static async Task ProcessAppCsProj(FileInfo csProjFile, ColorInfo primaryColor)
        {
            string csProjContent = await File.ReadAllTextAsync(csProjFile.FullName);

            StringBuilder newContent = new StringBuilder();
            int hexLength = primaryColor.Hex.Length;

            int startFrom = 0;
            string startTag = "<MauiIcon Include=\"Resources\\AppIcon\\appicon.svg\" ForegroundFile=\"Resources\\AppIcon\\appiconfg.svg\" Color=\"";
            int index = csProjContent.IndexOf(startTag, startFrom) + startTag.Length;
            newContent
                .Append(csProjContent.Substring(startFrom, index - startFrom))
                .Append(primaryColor.Hex)
                .And(_ => startFrom = index + hexLength);

            startTag = "<MauiSplashScreen Include=\"Resources\\Splash\\splash.svg\" Color=\"";
            index = csProjContent.IndexOf(startTag, startFrom) + startTag.Length;
            newContent
                .Append(csProjContent.Substring(startFrom, index - startFrom))
                .Append(primaryColor.Hex)
                .And(_ => startFrom = index + hexLength)
                ;

            newContent
                .Append(csProjContent.Substring(startFrom))
                ;

            await File.WriteAllTextAsync(csProjFile.FullName, newContent.ToString());
        }

        static async Task ProcessAppColorsXaml(FileInfo colorsXamlFile, ColorInfo primaryColor, ColorInfo primaryDarkColor, ColorInfo primaryDarkTextColor, ColorInfo secondaryColor, ColorInfo secondaryDarkTextColor, ColorInfo tertiaryColor)
        {
            string colorsXamlContent = await File.ReadAllTextAsync(colorsXamlFile.FullName);

            StringBuilder newContent = new StringBuilder();
            int hexLength = primaryColor.Hex.Length;

            int startFrom = 0;
            string startTag = "<Color x:Key=\"Primary\">";
            int index = colorsXamlContent.IndexOf(startTag, startFrom) + startTag.Length;
            newContent
                .Append(colorsXamlContent.Substring(startFrom, index - startFrom))
                .Append(primaryColor.Hex)
                .And(_ => startFrom = index + hexLength);
            ;

            startTag = "<Color x:Key=\"PrimaryDark\">";
            index = colorsXamlContent.IndexOf(startTag, startFrom) + startTag.Length;
            newContent
                .Append(colorsXamlContent.Substring(startFrom, index - startFrom))
                .Append(primaryDarkColor.Hex)
                .And(_ => startFrom = index + hexLength)
                ;

            startTag = "<Color x:Key=\"PrimaryDarkText\">";
            index = colorsXamlContent.IndexOf(startTag, startFrom) + startTag.Length;
            newContent
                .Append(colorsXamlContent.Substring(startFrom, index - startFrom))
                .Append(primaryDarkTextColor.Hex)
                .And(_ => startFrom = index + hexLength)
                ;

            startTag = "<Color x:Key=\"Secondary\">";
            index = colorsXamlContent.IndexOf(startTag, startFrom) + startTag.Length;
            newContent
                .Append(colorsXamlContent.Substring(startFrom, index - startFrom))
                .Append(secondaryColor.Hex)
                .And(_ => startFrom = index + hexLength)
                ;

            startTag = "<Color x:Key=\"SecondaryDarkText\">";
            index = colorsXamlContent.IndexOf(startTag, startFrom) + startTag.Length;
            newContent
                .Append(colorsXamlContent.Substring(startFrom, index - startFrom))
                .Append(secondaryDarkTextColor.Hex)
                .And(_ => startFrom = index + hexLength)
                ;

            startTag = "<Color x:Key=\"Tertiary\">";
            index = colorsXamlContent.IndexOf(startTag, startFrom) + startTag.Length;
            newContent
                .Append(colorsXamlContent.Substring(startFrom, index - startFrom))
                .Append(tertiaryColor.Hex)
                .And(_ => startFrom = index + hexLength)
                ;


            newContent
                .Append(colorsXamlContent.Substring(startFrom))
                ;

            await File.WriteAllTextAsync(colorsXamlFile.FullName, newContent.ToString());
        }
    }
}
