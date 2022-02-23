using Bridge.Html5;
using System.Linq;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public static class Navi
    {
        public static void Go(params object[] pathAndQueryParts)
        {
            Window.Location.Hash = CurateUrl(pathAndQueryParts);
        }

        public static void GoToLogin(string returnTo = null)
            => Go(
                    "login",
                    string.IsNullOrWhiteSpace(returnTo) ? null : $"?returnTo={Window.EncodeURIComponent(returnTo)}"
                );

        public static void GoHome()
            => Go(string.Empty);

        public static void GoToUnauthorized(string pageRef = null)
            => Go(
                    "nogo",
                    string.IsNullOrWhiteSpace(pageRef) ? null : $"?ref={Window.EncodeURIComponent(pageRef)}"
                );

        public static void ChangeDisplayedHash(params object[] pathAndQueryParts)
        {
            using (new ScopedRunner
                (
                onStart: AppNavigationBootstrapper.PauseHashNavigation,
                onStop: AppNavigationBootstrapper.ResumeHashNavigation
                )
            )
            {
                Window.Location.Hash = CurateUrl(pathAndQueryParts);
            }
        }

        private static string CurateUrl(object[] pathAndQueryParts)
        {
            string[] curatedParts
                = pathAndQueryParts
                ?.Where(x => !string.IsNullOrWhiteSpace(x?.ToString()))
                .Select(x => (x.ToString().First() == '/' ? x.ToString().Substring(1) : x.ToString()).Trim())
                .ToArray()
                ?? new string[0]
                ;

            string[] pathParts
                = curatedParts
                .Where(x => x.FirstOrDefault().NotIn('?', '&'))
                .ToArray()
                ;

            string[] queryParts
                = curatedParts
                .Where(x => x.FirstOrDefault().In('?', '&'))
                .Select(x => x.Substring(1))
                .ToArray()
                ;

            string url = $"/{string.Join("/", pathParts)}";
            if (queryParts.Any())
                url += $"?{string.Join("&", queryParts)}";

            return url;
        }
    }
}
