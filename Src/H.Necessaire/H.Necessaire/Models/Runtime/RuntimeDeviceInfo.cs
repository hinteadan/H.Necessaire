using System;

namespace H.Necessaire
{
    public class RuntimeDeviceInfo : IStringIdentity
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public RuntimeDeviceType Type { get; set; } = RuntimeDeviceType.Unknown;
        public string TypeName { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string BrandFullName { get; set; }
        public string Model { get; set; }
        public bool? HasTouchInteraction { get; set; }
        public bool? IsMobile { get; set; }

        public int? DisplayWidthInPixels { get; set; }
        public int? DisplayHeightInPixels { get; set; }

        public int? AppWidthInPixels { get; set; }
        public int? AppHeightInPixels { get; set; }

        public int? ColorDepthInBits { get; set; }
    }
}
