using System;
using System.IO;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public abstract class StreamedAuditEntryBase : AuditEntryBase
    {
        #region Construct
        readonly Func<ImAnAuditEntry, Task<Stream>> streamLoader;

        public StreamedAuditEntryBase(ImAnAuditEntry metadata, Func<ImAnAuditEntry, Task<Stream>> streamLoader) : base(metadata)
        {
            this.streamLoader = streamLoader;
        }

        protected abstract Task<OperationResult<T>> ReadAndParseStream<T>(Stream stream);
        #endregion

        public override async Task<T> GetObjectSnapshot<T>()
        {
            using (Stream stream = await streamLoader?.Invoke(this))
            {
                if (stream == null)
                    return default(T);

                OperationResult<T> parseResult = OperationResult.Fail().WithoutPayload<T>();

                await
                    new Func<Task>(async () =>
                    {
                        parseResult = await ReadAndParseStream<T>(stream);
                    })
                    .TryOrFailWithGrace(onFail: ex => parseResult = OperationResult.Fail(ex).WithoutPayload<T>());

                if (!parseResult.IsSuccessful)
                    return default(T);

                return parseResult.Payload;
            }
        }
    }
}
