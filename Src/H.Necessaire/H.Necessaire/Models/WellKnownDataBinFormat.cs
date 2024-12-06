namespace H.Necessaire
{
    public static class WellKnownDataBinFormat
    {
        public static DataBinFormatInfo
            GenericByteStream => new DataBinFormatInfo
            {
                ID = nameof(GenericByteStream),
                Extension = null,
                MimeType = "application/octet-stream",
                Encoding = null,
            };

        public static DataBinFormatInfo
            PlainTextUTF8 => new DataBinFormatInfo
            {
                ID = nameof(PlainTextUTF8),
                Extension = "txt",
                MimeType = "text/plain",
                Encoding = "utf-8",
            };

        public static DataBinFormatInfo
            JsonUTF8 => new DataBinFormatInfo
            {
                ID = nameof(JsonUTF8),
                Extension = "json",
                MimeType = "application/json",
                Encoding = "utf-8",
            };

        public static DataBinFormatInfo Default => GenericByteStream;
    }
}
