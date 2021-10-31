using System;
using System.Collections.Generic;

namespace H.Necessaire
{
    public class HasherFactory : ImADependency
    {
        public const string SimpleSecureHasher = nameof(SimpleSecureHasher);
        public const string RS512Hasher = nameof(RS512Hasher);

        #region Construct
        const string configKeyDefaultHasher = "DefaultHasher";

        static Dictionary<string, KeyValuePair<Type, ImAHasherEngine>> knownHashers = new Dictionary<string, KeyValuePair<Type, ImAHasherEngine>> {
            { SimpleSecureHasher, new KeyValuePair<Type, ImAHasherEngine>(typeof(SimpleSecureHasher), new SimpleSecureHasher()) },
            { RS512Hasher, new KeyValuePair<Type, ImAHasherEngine>(typeof(RS512Hasher), new RS512Hasher()) },
        };
        string defaultHasher = nameof(SimpleSecureHasher);

        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            RuntimeConfig config = dependencyProvider.Get<ImAConfigProvider>()?.GetRuntimeConfig() ?? dependencyProvider.Get<RuntimeConfig>();

            defaultHasher = config?.Get(configKeyDefaultHasher)?.Value?.ToString() ?? defaultHasher;
            if (!knownHashers.ContainsKey(defaultHasher))
                defaultHasher = nameof(SimpleSecureHasher);
        }
        #endregion

        public ImAHasherEngine GetDefaultHasher() => knownHashers[defaultHasher].Value;

        public OperationResult<ImAHasherEngine> GetHasher(string hasherName)
        {
            if (!knownHashers.ContainsKey(hasherName))
                return OperationResult.Fail($"Hasher '{hasherName}' is unknown").WithoutPayload<ImAHasherEngine>();

            return OperationResult.Win().WithPayload(knownHashers[hasherName].Value);
        }
    }
}
