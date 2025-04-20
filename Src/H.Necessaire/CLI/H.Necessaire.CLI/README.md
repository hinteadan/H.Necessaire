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
