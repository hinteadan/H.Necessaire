namespace H.Necessaire.ReactAppSample
{
    public static class AppConfig
    {
        public static readonly RuntimeConfig Default = new RuntimeConfig
        {
            Values = new[] {

                  "Copyright".ConfigWith("Copyright &copy; {year}. H.Necessaire React App Sample; by Hintea Dan Alexandru. All rights reserved.")

                //, "BaseUrl".ConfigWith("https://localhost")
                //, "BaseApiUrl".ConfigWith("https://localhost")

                , "Formatting".ConfigWith(
                      "DateAndTime".ConfigWith("ddd, MMM dd, yyyy 'at' HH:mm 'UTC'")
                    , "Date".ConfigWith("ddd, MMM dd, yyyy")
                    , "Time".ConfigWith("HH:mm")
                    , "Month".ConfigWith("yyyy MMM")
                    , "DayOfWeek".ConfigWith("dddd")
                    , "TimeStampThisYear".ConfigWith("MMM dd 'at' HH:mm")
                    , "TimeStampOtherYear".ConfigWith("MMM dd, yyyy 'at' HH:mm")
                    , "TimeStampIdentifier".ConfigWith("yyyyMMdd_HHmmss")
                )

                , "PageTitlePrefix".ConfigWith("H.Necessaire React App Sample")
                , "HomePagePath".ConfigWith("home")
                , "IsHomePageSecured".ConfigWith(false.ToString())
                , "Security".ConfigWith(
                      "LoginUrl".ConfigWith("/Security/Login")
                    , "RefreshUrl".ConfigWith("/Security/Refresh")
                )
                , "BffApiSyncRegistryRelativeUrl".ConfigWith("/sync/sync")
                , "SyncIntervalInSeconds".ConfigWith(10.ToString())
            },
        };
    }
}
