using System;
using System.Text;

namespace H.Necessaire
{
    public class DataBinFormatInfo : IStringIdentity
    {
        public string ID { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; } = "application/octet-stream";
        public string Encoding { get; set; } = "utf-8";
        public Encoding ParseEncoding(Encoding defaultTo = null)
        {
            if (string.IsNullOrWhiteSpace(Encoding))
                return defaultTo;

            Encoding result = defaultTo;

            new Action(() =>
            {
                result = System.Text.Encoding.GetEncoding(Encoding);
            })
            .TryOrFailWithGrace(
                onFail: ex => result = defaultTo
            );

            return result;
        }
    }
}
