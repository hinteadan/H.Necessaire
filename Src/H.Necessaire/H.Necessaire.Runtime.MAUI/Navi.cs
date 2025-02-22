namespace H.Necessaire.Runtime.MAUI
{
    public static class Navi
    {
        public static async Task Go(string route, IDictionary<string, object> queryParams = null)
        {
            if (queryParams.IsEmpty())
            {
                await Shell.Current.GoToAsync(route);
                return;
            }

            await Shell.Current.GoToAsync(route, queryParams);
        }

        public static async Task Go(Uri route, IDictionary<string, object> queryParams = null)
        {
            if (queryParams.IsEmpty())
            {
                await Shell.Current.GoToAsync(route);
                return;
            }

            await Shell.Current.GoToAsync(route, queryParams);
        }

        public static async Task GoBack(IDictionary<string, object> queryParams = null)
        {
            await Go("..", queryParams);
        }
    }
}
