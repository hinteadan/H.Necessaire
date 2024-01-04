using Google.Cloud.Firestore;
using H.Necessaire.Runtime.Google.FirestoreDB.Core.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core
{
    public class HsFirestoreQuerySnapshotEnumerable : IDisposableEnumerable<HsFirestoreDocument>
    {
        readonly IEnumerable<HsFirestoreDocument> hsFirestoreDocuments;
        readonly CollectionOfDisposables<IDisposable> disposables;
        public HsFirestoreQuerySnapshotEnumerable(QuerySnapshot querySnapshot, params IDisposable[] disposables)
        {
            this.hsFirestoreDocuments = querySnapshot.Select(x => x.ConvertTo<HsFirestoreDocument>());
            this.disposables = new CollectionOfDisposables<IDisposable>(disposables);
        }

        public IEnumerator<HsFirestoreDocument> GetEnumerator()
            => hsFirestoreDocuments.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => hsFirestoreDocuments.GetEnumerator();

        public void Dispose()
        {
            new Action(() =>
            {
                if (disposables?.Any() == true)
                    disposables.Dispose();
            })
            .TryOrFailWithGrace();
        }
    }
}
