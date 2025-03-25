```csharp
var cb = new CouchbaseInteractor("dev-play-db");

using (CouchbaseOperations scope = cb.NewOperationScope((nameof(DummyData))))
{
    var data = new DummyData();

    var saveResult = await scope.Save<DummyData, Guid>(data);

    var streamResult = await scope.StreamQuery<DummyData>(scope.SelectCustom<DummyData>(x => x.FirstName).Limit(1));

    using (var stream = streamResult.Payload)
    {
        foreach (DummyData dbData in stream)
        {

        }
    }
}
```

```csharp
var cb = new CouchbaseInteractor("dev-play-db");

using (CouchbaseOperations scope = cb.NewOperationScope((nameof(DummyData))))
{
    var query = scope.SelectCustom<long>(x => CouchbaseExtraQueryMethods.Count());

    var streamResult = await scope.StreamQuery<long>(query);

    using (var stream = streamResult.Payload)
    {
        var count = stream.Single();
    }
}
```

```csharp
var cb = new CouchbaseInteractor("dev-play-db");

using (CouchbaseOperations scope = cb.NewOperationScope((nameof(DummyData))))
{
    var query = scope.SelectAll<DummyData>().Where(x => x.CreatedAt >= DateTimeOffset.UtcNow.AddHours(-12));

    var page = await scope.LoadPage<DummyData>(query, new PageFilter { PageIndex = 0, PageSize = 5 });
}

```

### Blobs
```csharp
var cb = new CouchbaseInteractor("dev-play-db");

DataBin dataBin
    = new DataBinMeta { Name = "Test", Format = WellKnownDataBinFormat.PlainTextUTF8 }
    .ToBin(meta =>
    {
        return $"Test Content @ {DateTime.UtcNow}".ToStream().ToDataBinStream().AsTask();
    });

            

using (CouchbaseOperations scope = cb.NewOperationScope((nameof(DummyData))))
{
    var blobSaveResult = await scope.SaveBlob(dataBin);
}

using (CouchbaseOperations scope = cb.NewOperationScope((nameof(DummyData))))
{
    DataBin firstBlob = (await scope.StreamAllBlobs()).ThrowOnFailOrReturn().FirstOrDefault();
    string content = await (await firstBlob.OpenDataBinStream()).DataStream.ReadAsStringAsync(isStreamLeftOpen: false);
}

using (CouchbaseOperations scope = cb.NewOperationScope((nameof(DummyData))))
{
    var blobSaveResult = await scope.SaveBlob(dataBin);
    DataBin blob = (await scope.StreamAllBlobs()).ThrowOnFailOrReturn().FirstOrDefault();
    await scope.DeleteBlob(blob.ID);
}
```