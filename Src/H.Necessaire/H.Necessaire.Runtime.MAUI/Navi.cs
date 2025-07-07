namespace H.Necessaire.Runtime.MAUI
{
    public static class Navi
    {
        public static async Task Go(string route, params Note[] queryParams)
        {
            if (queryParams.IsEmpty())
            {
                await Shell.Current.GoToAsync(route);
                return;
            }

            await Shell.Current.GoToAsync(route, queryParams.ToDictionary(x => x.ID, x => x.Value as object));
        }

        public static async Task Go(Uri route, params Note[] queryParams)
        {
            if (queryParams.IsEmpty())
            {
                await Shell.Current.GoToAsync(route);
                return;
            }

            await Shell.Current.GoToAsync(route, queryParams.ToDictionary(x => x.ID, x => x.Value as object));
        }

        public static async Task GoBack(params Note[] queryParams)
        {
            await Go("..", queryParams);
        }
    }
}
