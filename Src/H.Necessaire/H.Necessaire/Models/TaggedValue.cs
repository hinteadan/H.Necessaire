using System;
using System.IO;

namespace H.Necessaire
{
    public class TaggedValue<TValue> : IStringIdentity
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Note[] Notes { get; set; }
        public TValue Value { get; set; }
    }

    public class TaggedStream : TaggedValue<Stream>, IDisposable
    {
        public Stream Stream => Value;
        public void Dispose()
        {
            if (Stream is null)
                return;

            new Action(Stream.Dispose).TryOrFailWithGrace();
        }
    }
}
