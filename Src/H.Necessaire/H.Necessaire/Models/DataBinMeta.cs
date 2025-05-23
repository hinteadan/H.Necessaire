﻿using System;
using System.Text;

namespace H.Necessaire
{
    public class DataBinMeta : IGuidIdentity
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime AsOf { get; set; } = DateTime.UtcNow;
        public DataBinFormatInfo Format { get; set; } = WellKnownDataBinFormat.GenericByteStream;
        public Note[] Notes { get; set; }

        public string GenerateUniqueFileName()
        {
            StringBuilder printer = new StringBuilder(ID.ToString());

            if (!(Format?.Extension).IsEmpty())
                printer.Append(Format.Extension.StartsWith(".") ? "" : ".").Append(Format.Extension);

            return printer.ToString().ToSafeFileName();
        }

        public string GenerateHumanFriendlyFileName()
        {
            StringBuilder printer
                = new StringBuilder($"{(string.IsNullOrWhiteSpace(Name) ? ID.ToString() : Name)}_AsOf_{AsOf.PrintTimeStampAsIdentifier()}".ToSafeFileName());

            if (!(Format?.Extension).IsEmpty())
                printer.Append(Format.Extension.StartsWith(".") ? "" : ".").Append(Format.Extension);

            return printer.ToString().ToSafeFileName();
        }
    }
}
