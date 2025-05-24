using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public class DataBinStream : Stream, ImADataBinStream
    {
        #region Construct
        readonly Stream stream;
        readonly CollectionOfDisposables<IDisposable> otherDisposables;
        public DataBinStream(Stream stream, params IDisposable[] otherDisposables)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream), $"The underlying stream cannot be null");
            this.otherDisposables = new CollectionOfDisposables<IDisposable>(otherDisposables);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            HSafe.Run(() => base.Dispose(disposing));

            HSafe.Run(() => stream.Dispose());
            HSafe.Run(() => otherDisposables.Dispose());
        }

        public Stream DataStream => stream;

        public override bool CanRead => stream.CanRead;

        public override bool CanSeek => stream.CanSeek;

        public override bool CanWrite => stream.CanWrite;

        public override long Length => stream.Length;

        public override long Position { get => stream.Position; set => stream.Position = value; }

        public override void Flush() => stream.Flush();

        public override int Read(byte[] buffer, int offset, int count) => stream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => stream.Seek(offset, origin);

        public override void SetLength(long value) => stream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => stream.Write(buffer, offset, count);

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            => stream.BeginRead(buffer, offset, count, callback, state);

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            => stream.BeginWrite(buffer, offset, count, callback, state);

        public override bool CanTimeout => stream.CanTimeout;

        public override void Close() => stream.Close();
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
            => stream.CopyToAsync(destination, bufferSize, cancellationToken);

        [Obsolete]
        protected override WaitHandle CreateWaitHandle() => (WaitHandle)typeof(Stream).GetMethod(nameof(CreateWaitHandle)).Invoke(stream, new object[0]);

        public override int EndRead(IAsyncResult asyncResult)
        {
            return stream.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            stream.EndWrite(asyncResult);
        }

        public override bool Equals(object obj)
        {
            return stream.Equals(obj);
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return stream.FlushAsync(cancellationToken);
        }

        public override int GetHashCode()
        {
            return stream.GetHashCode();
        }

        public override object InitializeLifetimeService()
        {
            return stream.InitializeLifetimeService();
        }

        [Obsolete]
        protected override void ObjectInvariant()
        {
            typeof(Stream).GetMethod(nameof(ObjectInvariant)).Invoke(stream, new object[0]);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return stream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override int ReadByte()
        {
            return stream.ReadByte();
        }

        public override int ReadTimeout { get => stream.ReadTimeout; set => stream.ReadTimeout = value; }

        public override string ToString()
        {
            return stream.ToString();
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return stream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override void WriteByte(byte value)
        {
            stream.WriteByte(value);
        }

        public override int WriteTimeout { get => stream.WriteTimeout; set => stream.WriteTimeout = value; }
    }
}
