namespace H.Necessaire.Runtime.MAUI.Extensions
{
    public static class TextFormattingExtensions
    {
        public static FormattedString Add(this FormattedString formattedString, string value, Action<Span> formatter = null)
        {
            if (value.IsEmpty())
                return formattedString;

            if (formattedString is null)
                return value;

            formattedString.Spans.Add(new Span
            {
                Text = value,
            }.AndIf(formatter is not null, formatter));

            return formattedString;
        }
    }
}
