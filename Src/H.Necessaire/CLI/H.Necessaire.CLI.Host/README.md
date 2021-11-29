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

# Copy BridgeDotNet App

_.\H.Necessaire.CLI.exe_ 

**BridgeDotNet copy** 

"Src=C:\H\H.Necessaire\Src\H.Necessaire\Apps\H.Necessaire.ReactAppSample"

"Dst=C:\H\H.Necessaire\Src\H.Necessaire\Apps\H.Necessaire.AspNetCoreWebAppSample"

"Dst=C:\H\H.Necessaire\Src\H.Necessaire\Apps\H.Necessaire.AspNetCoreWebAppSample2"

---

#### CMD Copy-paste:
```
.\H.Necessaire.CLI.exe bridgedotnet copy "Src=C:\H\H.Necessaire\Src\H.Necessaire\Apps\H.Necessaire.ReactAppSample" "Dst=C:\H\H.Necessaire\Src\H.Necessaire\Apps\H.Necessaire.AspNetCoreWebAppSample"
```