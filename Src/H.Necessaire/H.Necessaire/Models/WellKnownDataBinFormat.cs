using System;
using System.Collections.Generic;
using System.Text;

namespace H.Necessaire
{
    public static class WellKnownDataBinFormat
    {
        public static readonly DataBinFormatInfo
            GenericByteStream = new DataBinFormatInfo
            {
                ID = nameof(GenericByteStream),
                Extension = null,
                MimeType = "application/octet-stream",
                Encoding = null,
            };

        public static readonly DataBinFormatInfo
            PlainTextUTF8 = new DataBinFormatInfo
            {
                ID = nameof(PlainTextUTF8),
                Extension = "txt",
                MimeType = "text/plain",
                Encoding = "utf-8",
            };

        public static readonly DataBinFormatInfo
            JsonUTF8 = new DataBinFormatInfo
            {
                ID = nameof(JsonUTF8),
                Extension = "json",
                MimeType = "application/json",
                Encoding = "utf-8",
            };

        public static DataBinFormatInfo Default => GenericByteStream;
    }
}
