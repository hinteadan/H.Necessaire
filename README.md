![H.Necessaire Logo](https://raw.githubusercontent.com/hinteadan/H.Necessaire/master/Branding/IconLogo.png)
# H's Necessaire
_**Data and operation extensions for faster .NET (standard2.0) development**_

[![Nuget](https://img.shields.io/nuget/v/H.Necessaire?label=NuGet)](https://www.nuget.org/packages/H.Necessaire) [![Nuget](https://img.shields.io/nuget/dt/H.Necessaire?label=NuGet%20Downloads)](https://www.nuget.org/packages/H.Necessaire) [![GitHub](https://img.shields.io/github/license/hinteadan/H.Necessaire?label=License)](https://github.com/hinteadan/H.Necessaire/blob/master/LICENSE) [![GitHub last commit](https://img.shields.io/github/last-commit/hinteadan/H.Necessaire?label=Latest%20commit)](https://github.com/hinteadan/H.Necessaire/commits/master) [![CodeFactor Grade](https://img.shields.io/codefactor/grade/github/hinteadan/H.Necessaire/master?label=Code%20Quality)](https://www.codefactor.io/repository/github/hinteadan/h.necessaire) [![Build Status](https://dev.azure.com/hinteadan/H.Necessaire/_apis/build/status/H.Necessaire%20Build?branchName=master)](https://dev.azure.com/hinteadan/H.Necessaire/_build/latest?definitionId=6&branchName=master)


This library groups together various data and operation extensions that I've used and I'm still using for faster .NET development.
This document describes all these items along with their use case description and some code samples.

They are grouped in three main areas, each with its own set of functional areas:

- [H's Necessaire](#hs-necessaire)
  - [Models](#models)
    - [`OperationResult`](#operationresult)
      - [Definition overview](#definition-overview)
      - [Use Case code sample](#use-case-code-sample)
    - [`OperationResultException`](#operationresultexception)
      - [Definition overview](#definition-overview-1)
    - [`Note`](#note)
      - [Definition overview](#definition-overview-2)
    - [`VersionNumber`](#versionnumber)
      - [Definition overview](#definition-overview-3)
    - [`Version`](#version)
      - [Definition overview](#definition-overview-4)
    - [`NumberInterval`](#numberinterval)
      - [Definition overview](#definition-overview-5)
    - [`IDisposableEnumerable`](#idisposableenumerable)
      - [Definition overview](#definition-overview-6)
    - [`ILimitedEnumerable`](#ilimitedenumerable)
      - [Definition overview](#definition-overview-7)
    - [`CollectionOfDisposables`](#collectionofdisposables)
      - [Definition overview](#definition-overview-8)
      - [Use Case code sample](#use-case-code-sample-1)
    - [`IGuidIdentity`](#iguididentity)
      - [Definition overview](#definition-overview-9)
    - [`IKeyValueStorage`](#ikeyvaluestorage)
      - [Definition overview](#definition-overview-10)
    - [`SortFilter`](#sortfilter)
      - [Definition overview](#definition-overview-11)
    - [`ISortFilter`](#isortfilter)
      - [Definition overview](#definition-overview-12)
    - [`SortFilterBase`](#sortfilterbase)
      - [Definition overview](#definition-overview-13)
    - [`PageFilter`](#pagefilter)
      - [Definition overview](#definition-overview-14)
    - [`IPageFilter`](#ipagefilter)
      - [Definition overview](#definition-overview-15)
    - [`Page`](#page)
      - [Definition overview](#definition-overview-16)
      - [Use Case code sample](#use-case-code-sample-2)
  - [Extensions](#extensions)
    - [AzureExtensions](#azureextensions)
      - [`DateTime.ToAzureTableStorageSafeMinDate`](#datetimetoazuretablestoragesafemindate)
      - [`DateTime.ToNetMinDateFromAzureTableStorageMinDate`](#datetimetonetmindatefromazuretablestoragemindate)
        - [Use Case code sample](#use-case-code-sample-3)
    - [CollectionExtensions](#collectionextensions)
      - [`item.In(collection)`](#itemincollection)
      - [`item.NotIn(collection)`](#itemnotincollection)
        - [Use Case code sample](#use-case-code-sample-4)
      - [`collection.Batch(int batchSize)`](#collectionbatchint-batchsize)
        - [Use Case code sample](#use-case-code-sample-5)
      - [`item.AsArray()`](#itemasarray)
        - [Use Case code sample](#use-case-code-sample-6)
      - [`array.Jump(int numberOfElementsToJump)`](#arrayjumpint-numberofelementstojump)
        - [Use Case code sample](#use-case-code-sample-7)
      - [`keywords.TrimToValidKeywordsOnly(int minLength, int maxNumberOfKeywords)`](#keywordstrimtovalidkeywordsonlyint-minlength-int-maxnumberofkeywords)
        - [Use Case code sample](#use-case-code-sample-8)
    - [DataExtensions](#dataextensions)
      - [`number.TrimToPercent()`](#numbertrimtopercent)
        - [Use Case code sample](#use-case-code-sample-9)
      - [`string.ParseToGuidOrFallbackTo()`](#stringparsetoguidorfallbackto)
      - [`string.ParseToIntOrFallbackTo()`](#stringparsetointorfallbackto)
        - [Use Case code sample](#use-case-code-sample-10)
      - [`DateTime.IsBetweenInclusive(from, to)`](#datetimeisbetweeninclusivefrom-to)
        - [Use Case code sample](#use-case-code-sample-11)
      - [`value.And(action)`](#valueandaction)
        - [Use Case code sample](#use-case-code-sample-12)
      - [`type.IsSameOrSubclassOf(otherType)`](#typeissameorsubclassofothertype)
        - [Use Case code sample](#use-case-code-sample-13)
      - [`dateTime.ToUnixTimestamp()`](#datetimetounixtimestamp)
        - [Use Case code sample](#use-case-code-sample-14)
      - [`long.UnixTimeStampToDateTime()`](#longunixtimestamptodatetime)
        - [Use Case code sample](#use-case-code-sample-15)
      - [`dateTime.EnsureUtc()`](#datetimeensureutc)
        - [Use Case code sample](#use-case-code-sample-16)
      - [`collection.AsDisposableEnumerable()`](#collectionasdisposableenumerable)
        - [Use Case code sample](#use-case-code-sample-17)
    - [ExceptionExtensions](#exceptionextensions)
      - [`exception.Flatten()`](#exceptionflatten)
        - [Use Case code sample](#use-case-code-sample-18)
      - [`exceptions.ToNotes()`](#exceptionstonotes)
        - [Use Case code sample](#use-case-code-sample-19)
    - [FileSystemExtensions](#filesystemextensions)
      - [`string.ToSafeFileName()`](#stringtosafefilename)
        - [Use Case code sample](#use-case-code-sample-20)
    - [TaskExtensions](#taskextensions)
      - [`value.AsTask()`](#valueastask)
        - [Use Case code sample](#use-case-code-sample-21)
      - [`action.AsAsync()`](#actionasasync)
        - [Use Case code sample](#use-case-code-sample-22)
  - [Operations](#operations)
    - [`DataNormalizer`](#datanormalizer)
      - [Definition overview](#definition-overview-17)
      - [Use Case code sample](#use-case-code-sample-23)
    - [`Debouncer`](#debouncer)
      - [Definition overview](#definition-overview-18)
      - [Use Case code sample](#use-case-code-sample-24)
    - [`Throttler`](#throttler)
      - [Definition overview](#definition-overview-19)
      - [Use Case code sample](#use-case-code-sample-25)
    - [ExecutionUtilities](#executionutilities)
      - [`TryAFewTimesOrFailWithGrace(action)`](#tryafewtimesorfailwithgraceaction)
      - [`action.TryOrFailWithGrace()`](#actiontryorfailwithgrace)
        - [Use Case code sample](#use-case-code-sample-26)
    - [`ScopedRunner`](#scopedrunner)
      - [Definition overview](#definition-overview-20)
      - [Use Case code sample](#use-case-code-sample-27)
    - [`TimeMeasurement`](#timemeasurement)
      - [Definition overview](#definition-overview-21)
      - [Use Case code sample](#use-case-code-sample-28)
  - [Discussions](#discussions)

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
### `IDisposableEnumerable`
Used for modelling various use cases when you need to iterate on stuff that are bound to specific context.
Best example is iterating over a database stream, when you need to free the connection or any other type of unit of work created to access those external resources.
#### Definition overview
```csharp
interface IDisposableEnumerable<T> : IEnumerable<T>, IDisposable
```
---
### `ILimitedEnumerable`
Used for modelling limited collections. For instance when doing pagination or virtual pagination on a huge data set.
#### Definition overview
```csharp
interface ILimitedEnumerable<T> : IEnumerable<T>
{
  int Offset
  int Length
  int TotalNumberOfItems
}
```
---
### `CollectionOfDisposables`
The naming is super clear. It's used for working with a collection of disposable objects so that you don't need to handle each one's disposal.
For instance when aggregating something from multiple streams.
#### Definition overview
```csharp
class CollectionOfDisposables<T> : IDisposableEnumerable<T> where T : IDisposable
{
  //Constructs
  CollectionOfDisposables(params T[] disposables)
  CollectionOfDisposables(IEnumerable<T> disposables)
}
```
#### Use Case code sample
```csharp
using(var streams = new CollectionOfDisposables(File.OpenRead(@"C:\a.txt"), File.OpenRead(@"C:\b.txt")))
{
  //Process streams
}
```
---
### `IGuidIdentity`
Models and object that's identified by a `Guid`. Usage is obvious.
#### Definition overview
```csharp
interface IGuidIdentity
{
  Guid ID
}
```
---
### `IKeyValueStorage`
Operation contract for a key-values storage resource. Use case is obvious.
#### Definition overview
```csharp
interface IKeyValueStorage
{
  string StoreName { get; }
  Task Set(string key, string value, TimeSpan? validFor = null);
  Task Set(string key, string value, DateTime? validUntil = null);
  Task<string> Get(string key);
  Task Zap(string key);
  Task Remove(string key);//Should be just an alias for Zap. Most devs are not used to the verb "zap"
}
```
---
### `SortFilter`
Model used to define a sort criteria
#### Definition overview
```csharp
class SortFilter
{
  string By
  SortDirection Direction
  enum SortDirection
  {
    Ascending = 0,
    Descending = 1,
  }
}
```
---
### `ISortFilter`
Operation Contract for sorting
#### Definition overview
```csharp
interface ISortFilter
{
  SortFilter[] SortFilters
  OperationResult ValidateSortFilters();
}
```
---
### `SortFilterBase`
An abstract implementation of `ISortFilter` to make the use of it easier. When using this class you just need to override the `ValidSortNames`.
#### Definition overview
```csharp
abstract class SortFilterBase : ISortFilter
{
  protected abstract string[] ValidSortNames { get; }
}
```
---
### `PageFilter`
Model for pagination criteria.
#### Definition overview
```csharp
class PageFilter
{
  int PageIndex
  int PageSize
}
```
---
### `IPageFilter`
Operation contract for pagination
#### Definition overview
```csharp
interface IPageFilter
{
  PageFilter PageFilter
}
```
---
### `Page`
Model for a pagination operation result
#### Definition overview
```csharp
class Page<T>
{
  T[] Content
  int PageIndex
  int PageSize
  int? TotalNumberOfPages
  static Page<T> Single(params T[] content)
  static Page<T> Empty(int pageIndex = 0, int pageSize = 0, int totalNumberOfPages = 1)
}
```
#### Use Case code sample
```csharp
class UserFilter : SortFilterBase, IPageFilter { [...] }
Page<User> usersPage = await userResource.Browse(new UserFilter([...]));
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
#### `keywords.TrimToValidKeywordsOnly(int minLength, int maxNumberOfKeywords)`
Useful when doing a wildcard search to exclude irrelevant entries and to minimize the max number of search keys and avoid performance issues or overflows.
##### Use Case code sample
```csharp
string[] keywordsToSearchFor = filter.Customer?.Keywords?.TrimToValidKeywordsOnly();
```
---
### DataExtensions
Trimmers, parsers, fluent syntaxes, etc.
#### `number.TrimToPercent()`
Used for percent values, mainly for UI projections to make sure you don't display nasty percent value, like -13% or 983465.
##### Use Case code sample
```csharp
string ownershipPercentage = $"{Math.Round(OwnershipPercentage.Value.TrimToPercent(), 1)}%");
```
---
#### `string.ParseToGuidOrFallbackTo()`
&
#### `string.ParseToIntOrFallbackTo()`
Used as sugar syntax for `if(TryParse(out val)){[...]}`
##### Use Case code sample
```csharp
Guid? id = "asdasd".ParseToGuidOrFallbackTo(null);
```
---
#### `DateTime.IsBetweenInclusive(from, to)`
Sugar syntax for `date >= from && date <= to`
##### Use Case code sample
```csharp
if(!date.IsBetweenInclusive(DateTime.Now.AddMonths(-6), DateTime.Now))
    throw new InvalidOperationException("Selected date must in between today and six month ago");
```
---
#### `value.And(action)`
Used for fluent syntax. 
##### Use Case code sample
```csharp
Customer customer = new Customer()
    .And(c => c.FirstName = "Hin")
    .And(c => c.LastName = "Tee")
    .And(c => c.ID = Guid.NewGuid())
```
---
#### `type.IsSameOrSubclassOf(otherType)`
Used to check if a given type **is a derivate or same type** as the other.  Sugar syntax for `typeToCheck == typeToCompareWith || typeToCompareWith.IsSubclassOf(typeToCheck)`.
##### Use Case code sample
```csharp
class Customer : User { }
typeof(Customer).IsSameOrSubclassOf(typeof(User));//true
```
---
#### `dateTime.ToUnixTimestamp()`
Get the UNIX timestamp of a dateTime. Useful for compatibility with external systems, legacy code, cross-platform integration, JS integration
##### Use Case code sample
```csharp
var unixStamp = DateTime.Now.ToUnixTimestamp()
```
---
#### `long.UnixTimeStampToDateTime()`
Get the DateTime out of a UNIX timestamp. Useful for compatibility with external systems, legacy code, cross-platform integration, JS integration
##### Use Case code sample
```csharp
DateTime dateTime = 21763457162.UnixTimeStampToDateTime()
```
---
#### `dateTime.EnsureUtc()`
Makes sure that a given time is UTC so that you don't get unexpected behavior from `DateTime.Kind.Unspecified` or when persisting or serializing date-times.
##### Use Case code sample
```csharp
DateTime utcDate = DateTime.Now.EnsureUtc();
```
---
#### `collection.AsDisposableEnumerable()`
Converts an `IEnumerable<T>` into an `IDisposableEnumerable<T>`. This is just an in-memory wrapper over the enumeration, useful for in-memory mocks of certain resources. For real-world use-cases this shouldn't be used. The resource should always implement `IDisposableEnumerable<T>` itself
##### Use Case code sample
```csharp
int[] numbers = new [] { 1, 2, 3 };
IDisposableEnumerable<User> = numbers.AsDisposableEnumerable();
```


---
### ExceptionExtensions
Some useful extension methods for C# exceptions.
#### `exception.Flatten()`
Flattens the given exception instance. In short it recursively maps the root Exception plus any **InnerExceptions** and **AggregateExceptions** to a **flat array** of Exceptions.
Super useful for logging the actual errors when dealing with Tasks, RPCs, DB access, because these scenarios usually hold the actual exception cause inside inner exceptions tree or aggregate exceptions.
##### Use Case code sample
```csharp
Exception[] allExceptions = aggExOrExWithInnerExOrCombinationOfBoth.Flatten();
```
---
#### `exceptions.ToNotes()`
Converts the given `exceptions` to `Note[]`. Useful for persistence.
##### Use Case code sample
```csharp
Note[] exceptions = aggregateException.Flatten().ToNotes();
```


---
### FileSystemExtensions
Some useful extension methods dealing with file names.
#### `string.ToSafeFileName()`
Converts the given string value to a new string which can safely be used as file name.
##### Use Case code sample
```csharp
string fileName = "bubu?*:.txt".ToSafeFileName();//gets converted into bubu.txt
File.WriteAllText($"C:\Users\bubu\Downloads\{fileName}", "File Content");
```

---
### TaskExtensions
Sugar syntaxes for Task API.
#### `value.AsTask()`
Sugar syntax for `Task.FromResult(value);`
Useful mainly when implementing in-memory mocks for interfaces that use the Task API.
##### Use Case code sample
```csharp
interface ImAResource
{
    Task ShowMeTheMoney();
}
class InMemoryResource : ImAResource
{
    public Task<int> ShowMeTheMoney()
    {
        return 199.AsTask();
    }
}
```
---
#### `action.AsAsync()`
Transforms a synchronous `Action` to an asynchronous `Func<Task>` so you can safely use it along with the Task API.
##### Use Case code sample
```csharp
interface ImAnEngine
{
    Task RunDelayed(Func<Task> thisLogic);
}
[...]
void ReportProgress()
{
    Console.WriteLine("Progress...");
}
[...]
await myEngine.RunDelayed(ReportProgress.AsAsync())
```


## Operations
Data normalizer, unsafe code execution, debounce, throttle, execution time measurement, to name a few. See the full list below.

---
### `DataNormalizer`
This is used to reduce or expand numeric intervals.
For instance you want to reduce a range of values between 100 and 10000 to 0 and 100, so you can present them as a percent.
#### Definition overview
```csharp
class DataNormalizer
{
    //Construct
    DataNormalizer(NumberInterval fromInterval, NumberInterval toInterval)

    //Translates the given value from fromInterval to toInterval
    double Do(double value);
}
```
#### Use Case code sample
```csharp
var files = System.IO.Directory.EnumerateFiles(@"C:\Users\bubu\Downloads");
DataNormalizer percentNormalizer = new DataNormalizer(new NumberInterval(0, files.Count()), new NumberInterval(0, 100));
int fileIndex = 0;
foreach(var file in files)
{
    await Backup(file);
    fileIndex++;
    await ReportProgress(percentNormalizer.Do(fileIndex));
}
```
---
### `Debouncer`
It is used to make sure that you invoke a specific action **only once** if sequential calls are made **in a given time frame**.
For instance I want to make sure that `DoThis()` is called just once even if it is repeatedly invoked 10 times in 10 seconds.
#### Definition overview
```csharp
class Debouncer
{
    //Construct
    Debouncer(Func<Task> asyncActionToTame, TimeSpan debounceInterval)

    Task Invoke();

    Dispose();
}
```
#### Use Case code sample
```csharp
Task SearchUsers(string key){ [...] }

//If the user types faster than 1 char / second, we make sure to invoke SearchUsers just once, after he's done typing
var debouncedUserSearch = new Debouncer(SearchUsers, TimeSpan.FromSeconds(1))

async Task OnSearchInputTextChanged(TextInput textInput)
{
    [...]
    await debouncedUserSearch.Invoke();
    [...]
}
```
---
### `Throttler`
It is used to make sure that you invoke a specific action **only once** every **given time frame** while that action is continuously being invoked.
For instance I do a super frequent action (e.g.: a file iteration on a huge file tree) but I want to report progress only once a second.
This is very similar with the `Debouncer` except that the execution is not fully debounced to the end of the sequential invocations, but also during them.
#### Definition overview
```csharp
class Throttler
{
    //Construct
    Throttler(Func<Task> asyncActionToTame, TimeSpan throttleInterval)

    Task Invoke();
}
```
#### Use Case code sample
```csharp
Task ReportProgress(){ [...] }

//We only want to report progress once a second, no matter how fast an operation goes
var throttledProgressReport = new Throttler(ReportProgress, TimeSpan.FromSeconds(1))

async Task BackupFiles()
{
    [...]
    foreach(var file in filesToBackup)
    {
        [...]
        await throttledProgressReport.Invoke();
        [...]
    }
}
```
---
### ExecutionUtilities
Extensions that help with the execution of unsafe code that we don't want to crash our app or to stop the execution flow.
A good example is the interaction with external resources.
Say we want to raise a web-hook but we don't want our app to crash if that endpoint is not available but rather fallback to another mechanism.
#### `TryAFewTimesOrFailWithGrace(action)`
&
#### `action.TryOrFailWithGrace()`
These are the methods used to achieve this behavior. You can see them as a sugar syntax over `try{}catch{}` plus a **retry policy**.
`TryOrFailWithGrace` is just an extension method wrapper over the `static TryAFewTimesOrFailWithGrace`.
##### Use Case code sample
```csharp
Task CallExternalWebHook() { [...] }

async Task ProcessCustomerRequest()
{
    [...]
    await 
        CallExternalWebHook
        .TryOrFailWithGrace(
            numberOfTimes: 3,
            onFail: async ex => await LogExceptionAndNotifyAdmins(ex)//This is safely called as well; if an exception is thrown, the execution will continue
        )
}
```
---
### `ScopedRunner`
It's used to make sure a certain piece of code is executed event if 'quick return' occurs in the implementation. It does this by implementing `IDisposable` and thus leveraging the `using` syntax.
A good example here is setting a *waiting* state on a class.
#### Definition overview
```csharp
class ScopedRunner : IDisposable
{
    //Construct
    ScopedRunner(Action onStart, Action onStop)
    Dispose()
}
```
#### Use Case code sample
```csharp
using(new ScopedRunner(() => IsBusy = true, () => IsBusy = false))
{
    var stuffToDo = await GetStuffToDo();

    if(stuffToDo == null)
        return;

    await stuffToDo.Do();
}
```
---
### `TimeMeasurement`
This is a practical application of the `ScopedRunner` used to measure the execution time of a piece of code. Internally it uses a `ScopedRunner` and a `Stopwatch` to do the measurement.
#### Definition overview
```csharp
class TimeMeasurement : IDisposable
{
    //Construct
    TimeMeasurement(Action<TimeSpan> onDone)
    Dispose()
}
```
#### Use Case code sample
```csharp
void LogDuration(TimeSpan duration) { [...] }

using(new TimeMeasurement(LogDuration))
{
    await RunHeavyStuff();
}
```




## Discussions

Questions, ideas, talks? Ping me on [github](https://github.com/hinteadan/H.Necessaire/discussions).