using System;
using static Retyped.dexie;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class IndexedDBVersionInfo
    {
        public int Version { get; set; } = 0;
        public Func<Dexie.Version.storesConfig> ConstructDbVersionSchema { get; set; }
        public Dexie.Version.upgradeFn VersionMigrationProcessor { get; set; }
    }
}
