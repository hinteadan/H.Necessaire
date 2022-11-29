using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace H.Necessaire
{
    public class HasherFactory : ImADependency
    {
        public const string MD5Hasher = nameof(MD5Hasher);

        #region Construct
        const string configKeyDefaultHasher = "DefaultHasher";

        static readonly ConcurrentDictionary<string, KeyValuePair<Type, ImAHasherEngine>> knownHashers
            = new ConcurrentDictionary<string, KeyValuePair<Type, ImAHasherEngine>>()
            .And(x =>
            {
                x.Add(MD5Hasher, new KeyValuePair<Type, ImAHasherEngine>(typeof(MD5Hasher), new MD5Hasher()));
            })
            ;
        string defaultHasher = nameof(MD5Hasher);

        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            RuntimeConfig config = dependencyProvider.GetRuntimeConfig();

            defaultHasher = config?.Get(configKeyDefaultHasher)?.Value?.ToString() ?? defaultHasher;
            if (!knownHashers.ContainsKey(defaultHasher))
                defaultHasher = nameof(MD5Hasher);
        }
        #endregion

        public ImAHasherEngine GetDefaultHasher() => knownHashers[defaultHasher].Value;

        public OperationResult<ImAHasherEngine> GetHasher(string hasherName)
        {
            if (!knownHashers.ContainsKey(hasherName))
                return OperationResult.Fail($"Hasher '{hasherName}' is unknown").WithoutPayload<ImAHasherEngine>();

            return OperationResult.Win().WithPayload(knownHashers[hasherName].Value);
        }

        public HasherFactory RegisterOrUpdateHasher(string hasherName, ImAHasherEngine hasherEngine)
        {
            KeyValuePair<Type, ImAHasherEngine> entryValue
                = new KeyValuePair<Type, ImAHasherEngine>(hasherEngine.GetType(), hasherEngine);
            knownHashers.AddOrUpdate(hasherName, entryValue, (key, existing) => entryValue);
            return this;
        }
    }
}
