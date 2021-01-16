![H.Necessaire Logo](https://raw.githubusercontent.com/hinteadan/H.Necessaire/master/Branding/IconLogo.png)
# H's Necessaire
_**Data and operation extensions for faster .NET (standard2.0) development**_

[![Build Status](https://dev.azure.com/hinteadan/H.Necessaire/_apis/build/status/H.Necessaire%20Build?branchName=master)](https://dev.azure.com/hinteadan/H.Necessaire/_build/latest?definitionId=6&branchName=master)

This library groups together various data and operation extensions that I've used and I'm still using for faster .NET development.
This document describes all these items along with their use case description and some code samples.

They are grouped in three main areas, each with its own set of functional areas:

[TOC]

---

## Models
.NET Classes that can be generally used for various intents.

---

### `OperationResult`
Used to model any kind of operation on which you want to know whether it was **successful or not** and the **reason(s)** why.
#### Definition overview
```csharp
class OperationResult
{
    bool IsSuccessful
    string Reason
    string[] Comments
    ThrowOnFail()
    FlattenReasons()
}

class OperationResult<T> : OperationResult
{
    T Payload
    T ThrowOnFailOrReturn()
}
```
#### Use Case code sample
```csharp
public OperationResult<Order> ValidateOrder(Order order)
{
    if (order == null)
        return OperationResult.Fail("The order is inexistent").WithoutPayload<Order>();
    if (!order.Items.Any())
        return OperationResult.Fail("The order has no items").WithoutPayload<Order>();

    return OperationResult.Win().WithPayload(order);
}

[...]

OperationResult<Order> orderValidation = ValidateOrder(order);
if (!orderValidation.IsSuccessful)
{
    //log errors
    return;
}

SaveOrder(order);

[...]

Order order = ValidateOrder(order).ThrowOnFailOrReturn();
```

---

### `OperationResultException`
Extends `AggregateException` and is the exception type thrown by `OperationResult.ThrowOnFail()` or `OperationResult<T>.ThrowOnFailOrReturn()`.
You shouldn't manually instantiate this type of exception.
#### Definition overview
```csharp
class OperationResultException : AggregateException
{
    OperationResult OperationResult
}
```

---

### `Note`
Used instead of a string-string key value pair for lighter syntax.
#### Definition overview
```csharp
struct Note
{
    string Id
    string Value
    static Note[] FromDictionary(Dictionary<string, string> keyValuePairs)
}
```

---

### `VersionNumber`
Used to model [semantic versioning](https://semver.org/).
#### Definition overview
```csharp
class VersionNumber
{
    int Major
    int Minor
    int Patch
    int Build
    string Suffix

    string Semantic
}
```

---

### `Version`
Used to model version number correlation with code reference.
#### Definition overview
```csharp
class Version
{
    VersionNumber Number
    DateTime Timestamp
    string Branch
    string Commit
}
```

---

### `NumberInterval`
Used to model numeric intervals.
#### Definition overview
```csharp
struct NumberInterval
{
    double Min
    double Max
}
```

---

## Extensions
A bunch of helpful extension methods for collections, exceptions, tasks, Azure, and more. Just read down below.

---

### AzureExtensions
A bunch of helpful Azure extension methods.
#### `DateTime.ToAzureTableStorageSafeMinDate`
&
#### `DateTime.ToNetMinDateFromAzureTableStorageMinDate`
If you want to store a `DateTime.MinValue` in a storage account in Azure you'll get an exception. That's because the minimum date that azure storage supports is `new DateTime(1601, 1, 1)`;
##### Use Case code sample
```csharp
class MyTableEntity : ITableEntity
{
    [...]
    public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
    {
        [...]
        if (properties.ContainsKey(nameof(CreatedAt)))
            CreatedAt = properties[nameof(CreatedAt)].DateTime?.ToNetMinDateFromAzureTableStorageMinDate() ?? DateTime.MinValue;
        [...]
    }

    public static MyTableEntity FromMyEntity(MyEntity entity)
    {
        [...]
        CreatedAt = entity.CreatedAt.ToAzureTableStorageSafeMinDate(),
        [...]
    }
    [...]
}
```

---

### CollectionExtensions
Batch, containment, array-ing, trimming, etc.
#### `item.In(collection)`
&
#### `item.NotIn(collection)`
A simpler syntax for checking if an item exists in a collection or not.
##### Use Case code sample
```csharp
if (workItem.Status.In(ProcessingStatus.CompletedAndWaitingForApproval, ProcessingStatus.CompletedButRejected))
    return OrderStatus.Reviewing;
```
---
#### `collection.Batch(int batchSize)`
Splits a collection in batches of the specified size.
##### Use Case code sample
```csharp
foreach (IEnumerable<Command> batch in commands.Batch(4))
    result.AddRange(await Task.WhenAll(batch.Select(ProcessCommand).ToArray()));
```
---
#### `item.AsArray()`
Converts the given item into a one element array. Sugar syntax for `new [] { item }` or `new ItemType[] { item }`.
##### Use Case code sample
```csharp
var searchResults = await orderResource.SearchOrders(new OrderFilter
{
    CustomerIDs = request.Customer.ID.AsArray(),
    [...]
});
```
---
#### `array.Jump(int numberOfElementsToJump)`
Safely tries to create a new array without the first `numberOfElementsToJump` elements. An exception-less alternative to `.Skip()`.
##### Use Case code sample
```csharp
var result = new [] { 1, 2, 3 }.Jump(100); //No exception, result will be an empty array.
```
---
#### `keywords.TrimToValidKeywordsOnly(int minLength = 3, int maxNumberOfKeywords = 3)`
Useful when doing a wildcard search to exclude irrelevant entries and to minimize the max number to avoid performance or even overflow hits.
##### Use Case code sample
```csharp
string[] keywordsToSearchFor = filter.Customer?.Keywords?.TrimToValidKeywordsOnly();
```
---
### DataExtensions
_[Docs in progress]_

---
### ExceptionExtensions
_[Docs in progress]_

---
### FileSystemExtensions
_[Docs in progress]_

---
### TaskExtensions
_[Docs in progress]_


## Operations
Data normalizer, unsafe code execution, debounce, throttle, execution time measurement, to name a few. See the full list below.
_[Docs in progress]_



## Discussions

Questions, ideas, talks? Ping me on [github](https://github.com/hinteadan/H.Necessaire/discussions).