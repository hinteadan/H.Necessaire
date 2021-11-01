using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public static class AppNavigationExtensions
    {
        public static bool IsIndexPath(this string path)
        {
            Console.Write($"IsIndexPath({path})");

            path = path?.Trim();

            return
                string.IsNullOrWhiteSpace(path)

                || path == AppBase.BaseHostPath
                || path == $"{AppBase.BaseHostPath}/"
                || path == $"{AppBase.BaseHostPath}/#"
                || path == $"{AppBase.BaseHostPath}/#/"
                || path == $"{AppBase.BaseHostPath}/#//"
                || path == $"{AppBase.BaseHostPath}/#///"

                || path == "/"
                || path == "#" || path == "/#"
                || path == "#/" || path == "/#/"
                || path == "#//" || path == "/#//"
                || path == "#///" || path == "/#///"

                ;
        }
    }
}
