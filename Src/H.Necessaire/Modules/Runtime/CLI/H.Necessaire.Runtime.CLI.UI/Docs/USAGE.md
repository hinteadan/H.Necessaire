# Progress indicators

```csharp
using (await new string[] { "test1", "test2", "test3" }.CliUiDeterministicProgressScope())
{
    int steps = 5;

    ProgressReporter[] reporters = CliUiProgressiveScope.Current.ProgressReporters;

    foreach (ProgressReporter reporter in reporters)
    {
        reporter.SetSourceInterval(new NumberInterval { Min = 0, Max = steps });
    }

    await Task.WhenAll(reporters.Select(r => r.RaiseOnProgress("Starting", 0, "log a", "log b")));

    await Task.WhenAll(reporters.Select(async reporter =>
    {

        for (int i = 1; i <= steps; i++)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(Random.Shared.Next(100, 900)));

            await reporter.RaiseOnProgress($"{reporter.ID} Working {i} / {steps}", i, "log a", "log b");
        }

    }));

    await Task.WhenAll(reporters.Select(r => r.RaiseOnProgress($"{r.ID} DONE", steps)));
}

Log("ALL Done");

return OperationResult.Win();
```

```csharp
ImACliUI cliUi;
public override void ReferDependencies(ImADependencyProvider dependencyProvider)
{
    base.ReferDependencies(dependencyProvider);
    cliUi = dependencyProvider.Get<ImACliUI>();
}

[...]

using (await cliUi.ProgressiveScope("test1", "test2", "test3"))
{
    int steps = 5;

    ProgressReporter[] reporters = CliUiProgressiveScope.Current.ProgressReporters;

    foreach (ProgressReporter reporter in reporters)
    {
        reporter.SetSourceInterval(new NumberInterval { Min = 0, Max = steps });
    }

    await Task.WhenAll(reporters.Select(r => r.RaiseOnProgress("Starting", 0)));

    await Task.WhenAll(reporters.Select(async reporter =>
    {

        for (int i = 1; i <= steps; i++)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(Random.Shared.Next(100, 900)));

            await reporter.RaiseOnProgress($"{reporter.ID} Working {i} / {steps}", i);
        }

    }));

    await Task.WhenAll(reporters.Select(r => r.RaiseOnProgress($"{r.ID} DONE", steps)));
}

```

# User Input

```csharp
bool confirmed = await "Confirm?".CliUiAskForConfirmation();

OperationResult<string> = await "Name?".CliUiAskForAnyInput();

OperationResult<T> = await "Choice input?".CliUiAskForChoiceInput(choices);
```

# User Selection
```csharp
var selection = await "Select".CliUiAskForSingleSelection([1, 2, 3]);
```



# Printing

```csharp
exception.CliUiPrint();

markupString.CliUiPrintMarkup();

markupString.CliUiPrintMarkupLine();

stringToEscape.CliUiEscapeForMarkup();

jsonString.CliUiPrintJson();

Directory.GetCurrentDirectory().CliUiPrintPath();
```

# Widgets Printing

```csharp
new Note("test", "[blue]skjdhfladsf[/] safdahskjhfalkjdshflkdsaf").CliUiPrintPanel();

DateTime.Now.CliUiPrintCalendar(events: [new DateTime(2024, 12 ,25)]);
```