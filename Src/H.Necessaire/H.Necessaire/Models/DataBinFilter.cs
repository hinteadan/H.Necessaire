using System;

namespace H.Necessaire
{
    public class DataBinFilter : IDFilter<Guid>
    {
        protected override string[] ValidSortNames { get; } = new string[] { "ID", "Name", "AsOf" };

        public string[] Names { get; set; }
        public DateTime? FromInclusive { get; set; }
        public DateTime? ToInclusive { get; set; }
        public string[] FormatIDs { get; set; }
        public string[] FormatExtensions { get; set; }
        public string[] FormatMimeTypes { get; set; }
        public string[] FormatEncodings { get; set; }
        public Note[] Notes { get; set; }
    }
}
