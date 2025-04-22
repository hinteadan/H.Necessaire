# Update NuGet Version

_.\H.Necessaire.CLI.exe_ 

**nugetversion update** 

H.Necessaire=**patch**

H.Necessaire.Dapper=**minor**

H.Necessaire.WebSockets=**major** 

Newtonsoft.Json=**13.0.1**

---

#### CMD Copy-paste:
```
.\H.Necessaire.CLI.exe nugetversion update H.Necessaire=patch H.Necessaire.Dapper=minor H.Necessaire.WebSockets=major Newtonsoft.Json=13.0.1
```

_.\H.Necessaire.CLI.exe_ 

**nugetversion consolidate-deps**

Updates all external NuGet Deps in NuSpec with the version specified in CSPROJs

---

#### CMD Copy-paste:
```
.\H.Necessaire.CLI.exe nugetversion consolidate-deps
```


# Run external command

```csharp
ExternalCommandRunner externalCommandRunner;

public override void ReferDependencies(ImADependencyProvider dependencyProvider)
{
    base.ReferDependencies(dependencyProvider);
    externalCommandRunner = dependencyProvider.Get<ExternalCommandRunner>();
}

var x = await externalCommandRunner
    .WithContext(new ExternalCommandRunContext { IsOutputCaptured = true, IsOutputPrinted = false })
    .Run("node", "--version");
string result = x.Payload.OutputData.ToString().Trim();
var nodeVersion = VersionNumber.Parse(result);

OperationResult<ExternalCommandRunContext>[] results = await Task.WhenAll(
    externalCommandRunner.WithContext(new ExternalCommandRunContext { IsOutputCaptured = true }).RunCmd("tasklist"),
    externalCommandRunner.WithContext(new ExternalCommandRunContext { IsOutputCaptured = true }).RunCmd("dir")
);


return await externalCommandRunner.WithContext(new ExternalCommandRunContext { IsUserInputExpected = true }).RunCmd();

await externalCommandRunner
    .WithContext(new ExternalCommandRunContext
    {
        IsUserInputExpected = true,
        UserInputProvider = () => new string[] { "ping google.com", "exit" }.AsTask(),
    })
    .RunCmd();

externalCommandRunner.Run("some.exe", "with", "args");
```