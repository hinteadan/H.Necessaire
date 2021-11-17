using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public class SyncableBrowser : ImADependency, ImASyncableBrowser
    {
        #region Construct
        private static readonly Type[] syncableTypes = new Type[0];
        private static readonly Dictionary<Type, Type> syncerTypePerSyncableTypeDictionary = new Dictionary<Type, Type>();
        private static Dictionary<Type, ImASyncer> syncerPerSyncableTypeDictionary;

        static SyncableBrowser()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type[] allTypes = assemblies.SelectMany(assembly => assembly.GetTypes()).Where(x => x != null).ToArray();

            Type syncableInterfaceType = typeof(ImSyncable);
            Type guidIdentityType = typeof(IDentityType<Guid>);
            Type stringIdentityType = typeof(IDentityType<string>);
            Type syncerInterfaceType = typeof(ImASyncer);
            Type genericSyncerType = typeof(ImASyncer<,>);

            syncableTypes = allTypes.Where(p => { try { return p != null && !p.IsAbstract && syncableInterfaceType.IsAssignableFrom(p); } catch (Exception) { return false; } }).ToArray();

            Type[] syncerTypes
                = allTypes
                .Where(type => { try { return type.IsGenericType && !type.IsAbstract && syncerInterfaceType.IsAssignableFrom(type) && type.GetGenericTypeDefinition() == genericSyncerType; } catch (Exception) { return false; } })
                .ToArray();

            foreach (Type syncableType in syncableTypes)
            {
                Type identityType
                    = guidIdentityType.IsAssignableFrom(syncableType)
                    ? typeof(Guid)
                    : stringIdentityType.IsAssignableFrom(syncableType)
                    ? typeof(string)
                    : null
                    ;

                if (identityType == null)
                    throw new InvalidOperationException($"{syncableType.TypeName()} syncable type has an unsuppoerted ID type.");

                Type syncerType = genericSyncerType.MakeGenericType(new Type[] { syncableType, identityType });

                syncerTypePerSyncableTypeDictionary.Add(syncableType, syncerType);
            }
        }

        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            if (syncerPerSyncableTypeDictionary == null)
            {
                syncerPerSyncableTypeDictionary = new Dictionary<Type, ImASyncer>();
                foreach (KeyValuePair<Type, Type> syncerTypePerSyncable in syncerTypePerSyncableTypeDictionary)
                {
                    ImASyncer syncer = (ImASyncer)dependencyProvider.Get(syncerTypePerSyncable.Value);
                    if (syncer == null)
                        continue;

                    syncerPerSyncableTypeDictionary.Add(syncerTypePerSyncable.Key, syncer);
                }
            }
        }
        #endregion

        public virtual Task<Type[]> GetAllSyncableTypes()
        {
            return syncableTypes.AsTask();
        }

        public virtual async Task<object> LoadEntity(Type syncableType, string syncableInstanceID)
        {
            if (!syncerPerSyncableTypeDictionary.ContainsKey(syncableType))
            {
                return null;
            }

            ImASyncer syncer = syncerPerSyncableTypeDictionary[syncableType];

            return await syncer.LoadRaw(syncableInstanceID);
        }
    }
}
