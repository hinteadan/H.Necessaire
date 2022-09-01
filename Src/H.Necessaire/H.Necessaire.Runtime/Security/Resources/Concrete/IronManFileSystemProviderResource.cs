using H.Necessaire.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Security.Resources.Concrete
{
    public class IronManFileSystemProviderResource : ImADependency, ImTheIronManProviderResource
    {
        #region Construct
        UserInfo[] ironMen = null;
        IronManPass[] ironMenPasses = null;
        FileInfo ironmenFile = new FileInfo("ironmen.json");
        FileInfo ironmenPassFile = new FileInfo("ironmen.pass.json");
        FileSystemWatcher ironmenFileWatcher;
        FileSystemWatcher ironmenPassFileWatcher;
        ImALogger logger;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            logger = dependencyProvider.GetLogger<IronManFileSystemProviderResource>();
            RuntimeConfig config = dependencyProvider.GetRuntimeConfig();

            ironmenFile = new FileInfo(config?.Get("IronMen")?.Get("FilePath")?.ToString() ?? ironmenFile.FullName);
            ironmenPassFile = new FileInfo(config?.Get("IronMen")?.Get("PassFilePath")?.ToString() ?? ironmenPassFile.FullName);

            ironmenFileWatcher = new FileSystemWatcher(ironmenFile.Directory.FullName, ironmenFile.Name);
            ironmenFileWatcher.EnableRaisingEvents = true;
            ironmenFileWatcher.Changed += (s, e) => ironMen = null;

            ironmenPassFileWatcher = new FileSystemWatcher(ironmenPassFile.Directory.FullName, ironmenPassFile.Name);
            ironmenPassFileWatcher.EnableRaisingEvents = true;
            ironmenPassFileWatcher.Changed += (s, e) => ironMenPasses = null;
        }
        #endregion

        public async Task<UserInfo[]> GetIronMenByIds(params Guid[] ids)
        {
            await EnsureIronMen();

            return
                ironMen
                ?.Where(x => x.ID.In(ids))
                .ToArray()
                ;
        }

        public async Task<string> GetPasswordFor(Guid ironManUserID)
        {
            await EnsureIronMenPasses();

            return
                ironMenPasses
                ?.FirstOrDefault(x => x.ID == ironManUserID)
                ?.Password
                ;
        }

        public async Task<UserInfo[]> SearchIronMen(UserInfoSearchCriteria searchCriteria)
        {
            await EnsureIronMen();

            return
                ironMen
                ?.Where(x =>
                    (searchCriteria?.IDs?.Any() != true ? true : x.ID.In(searchCriteria.IDs))
                    && (searchCriteria?.Usernames?.Any() != true ? true : x.Username.In(searchCriteria.Usernames, (a, b) => string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase)))
                )
                .ToArray()
                ;
        }

        private async Task EnsureIronMen()
        {
            if (ironMen != null)
                return;

            if (!ironmenFile.Exists)
                ironMen = new UserInfo[0];

            await
                new Func<Task>(async () =>
                {
                    ironMen =
                        (await ironmenFile.OpenRead().ReadAsStringAsync(isStreamLeftOpen: false))
                        .TryJsonToObject(defaultTo: new UserInfo[0])
                        .ThrowOnFailOrReturn()
                        .Select(UserInfo.BuildIronMan)
                        .ToArray()
                        ;
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        ironMen = new UserInfo[0];
                        await logger.LogError($"Error occured while trying to load Iron Men from {ironmenFile.FullName}", ex);
                    }
                );
        }

        private async Task EnsureIronMenPasses()
        {
            if (ironMenPasses != null)
                return;

            if (!ironmenPassFile.Exists)
                ironMenPasses = new IronManPass[0];

            await
                new Func<Task>(async () =>
                {
                    ironMenPasses =
                        (await ironmenPassFile.OpenRead().ReadAsStringAsync(isStreamLeftOpen: false))
                        .TryJsonToObject(defaultTo: new IronManPass[0])
                        .ThrowOnFailOrReturn()
                        ;
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        ironMenPasses = new IronManPass[0];
                        await logger.LogError($"Error occured while trying to load Iron Men passwords from {ironmenPassFile.FullName}", ex);
                    }
                );
        }

        private class IronManPass
        {
            public Guid ID { get; set; }
            public string Password { get; set; }
        }
    }
}
