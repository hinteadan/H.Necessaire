namespace H.Necessaire.Runtime.UI
{
    public static class ViewModelExtensions
    {
        public static HViewModel<T> ToHViewModel<T>(this T data, string id, string title, string description, params Note[] notes)
            => new HViewModel<T>(id, title, description, notes).WithData(data);
        public static HViewModel<T> ToHViewModel<T>(this T data, string title, string description, params Note[] notes)
            => new HViewModel<T>(title, description, notes).WithData(data);
        public static HViewModel<T> ToHViewModel<T>(this T data, string title, params Note[] notes)
            => new HViewModel<T>(title, notes).WithData(data);
        public static HViewModel<T> ToHViewModel<T>(this T data, params Note[] notes)
            => new HViewModel<T>(notes).WithData(data);
        public static HViewModel<T> ToHViewModel<T>(this T data)
            => new HViewModel<T>().WithData(data);
    }
}
