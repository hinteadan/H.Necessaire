# CLI Progress reporting

```csharp
using (new ProgressiveScope("docs", (s, e) => { Log($"{e.PercentValue.ToString("0.00")} % Running Docs - {e.CurrentActionName}..."); return Task.CompletedTask; }))
{
    return await RunSubCommand();
}

[...]

using (new CodeAnalysisProgressiveScope((s, e) => { Log($"{e.PercentValue.ToString("0.00")} % Running Code Analysis - {e.CurrentActionName}..."); return Task.CompletedTask; }))
{
    return await RunSubCommand();
}

[...]

public class CodeAnalysisProgressiveScope : ProgressiveScope
{
    public CodeAnalysisProgressiveScope(AsyncEventHandler<ProgressEventArgs> onProgress = null) : base("CodeAnalysisProgressiveScope", onProgress)
    {
    }

    public static ProgressReporter GetReporter() => ProgressReporter.Get("CodeAnalysisProgressiveScope") ?? new ProgressReporter();
}
```


# Async Events

```csharp
readonly AsyncEventRaiser<ProgressEventArgs> progressRaiser;

//Construct
Constructor() {
    progressRaiser = new AsyncEventRaiser<ProgressEventArgs>(this);
}

//Declare event
public event AsyncEventHandler<ProgressEventArgs> OnProgress { add => progressRaiser.OnEvent += value; remove => progressRaiser.OnEvent -= value; }

//Raise
private async Task RaiseOnProgressAction(params string[] additionalInfo)
{
    await progressRaiser.Raise(new ProgressEventArgs(
        currentActionName,
        percentNormalizer.From,
        progressSourceValue,
        percentNormalizer.Do(progressSourceValue),
        additionalInfo
    ));
}
```