using System;
using System.IO;

namespace H.Necessaire
{
    internal class DefaultDataBinStream : ImADataBinStream
    {
        readonly CollectionOfDisposables<IDisposable> otherDisposables;
        public DefaultDataBinStream(Stream dataStream, params IDisposable[] otherDisposables)
        {
            DataStream = dataStream;
            this.otherDisposables = new CollectionOfDisposables<IDisposable>(otherDisposables);
        }

        public Stream DataStream { get; private set; }

        public void Dispose()
        {
            new Action(() =>
            {
                if(DataStream != null)
                    DataStream.Dispose();
                DataStream = null;
            })
            .TryOrFailWithGrace();

            new Action(() =>
            {
                if(otherDisposables != null)
                    otherDisposables.Dispose();
            })
            .TryOrFailWithGrace();
        }
    }
}
