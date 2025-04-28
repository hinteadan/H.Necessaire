using System;
using System.Collections.Generic;
using System.Text;

namespace H.Necessaire.Runtime.Sqlite.Core
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            //dependencyRegistry
            //    .Register<TestDataModelSqliteStorageResource>(() => new TestDataModelSqliteStorageResource())
            //    .Register<ImAStorageService<Guid, TestDataModel>>(() => dependencyRegistry.Get<TestDataModelSqliteStorageResource>())
            //    .Register<ImAStorageBrowserService<TestDataModel, TestDataModelFilter>>(() => dependencyRegistry.Get<TestDataModelSqliteStorageResource>())
            //    ;
        }
    }
}
