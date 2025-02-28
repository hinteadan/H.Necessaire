namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class PatchedTimePicker : TimePicker
    {
        const double windowsOffsetFix = 6.5;

        public PatchedTimePicker()
        {
#if WINDOWS
            Margin = new Thickness(0, windowsOffsetFix, 0, -windowsOffsetFix);
#endif
        }
    }
}
