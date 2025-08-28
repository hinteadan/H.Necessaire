using System;

namespace H.Necessaire
{
    public class HsHttpStreamResponse : HsHttpResponse, IDisposable
    {
        readonly CollectionOfDisposables<IDisposable> otherDisposables;
        public HsHttpStreamResponse(params IDisposable[] otherDisposables)
        {
            this.otherDisposables = new CollectionOfDisposables<IDisposable>(otherDisposables);
        }
        ~HsHttpStreamResponse() => HSafe.Run(Dispose);
        public void Dispose() => HSafe.Run(() => otherDisposables.Dispose());

        public DataBin Content { get; set; }
    }
}
