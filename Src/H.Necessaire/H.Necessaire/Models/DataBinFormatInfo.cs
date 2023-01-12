using System;
using System.Text;

namespace H.Necessaire
{
    public class DataBinFormatInfo : IStringIdentity, ICloneable
    {
        public string ID { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; } = "application/octet-stream";
        public string Encoding { get; set; } = "utf-8";

        public DataBinFormatInfo Clone()
        {
            return
                new DataBinFormatInfo
                {
                    ID = ID,
                    Encoding = Encoding,
                    Extension = Extension,
                    MimeType = MimeType,
                };
        }
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

        object ICloneable.Clone() => Clone();
    }
}
