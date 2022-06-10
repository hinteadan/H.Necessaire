using System;
using System.Linq;

namespace H.Necessaire
{
    public static class TypeExtensions
    {
        public static string TypeName(this object instance)
        {
            if (instance is Type)
                return (instance as Type).FullName;

            return instance.GetType().FullName;
        }

        public static bool IsSameOrSubclassOf(this Type typeToCheck, Type typeToCompareWith)
        {
            return
                typeToCheck == typeToCompareWith
                || typeToCompareWith.IsSubclassOf(typeToCheck);
        }

        public static Type[] GetAllImplementations(this Type baseType)
        {
            return
                AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(
                    assembly => assembly
                    .GetTypes()
                    .Where(
                        p =>
                        p != baseType
                        && baseType.IsAssignableFrom(p)
                        && !p.IsAbstract
                    )
                )
                .ToArray();
        }

        public static ImALogger GetLogger(this ImADependencyProvider dependencyProvider, string component, string application = "H.Necessaire")
            => dependencyProvider?.Get<ImALoggerFactory>()?.BuildLogger(component, dependencyProvider.GetRuntimeConfig()?.Get("App")?.Get("Name")?.ToString() ?? application);

        public static ImALogger GetLogger(this ImADependencyProvider dependencyProvider, Type type, string application = "H.Necessaire")
            => dependencyProvider?.Get<ImALoggerFactory>()?.BuildLogger(type.TypeName(), dependencyProvider.GetRuntimeConfig()?.Get("App")?.Get("Name")?.ToString() ?? application);

        public static ImALogger GetLogger<T>(this ImADependencyProvider dependencyProvider, string application = "H.Necessaire")
            => dependencyProvider?.Get<ImALoggerFactory>()?.BuildLogger<T>(dependencyProvider.GetRuntimeConfig()?.Get("App")?.Get("Name")?.ToString() ?? application);

        public static RuntimeConfig GetRuntimeConfig(this ImADependencyProvider dependencyProvider)
            => dependencyProvider?.Get<ImAConfigProvider>()?.GetRuntimeConfig() ?? dependencyProvider?.Get<RuntimeConfig>() ?? RuntimeConfig.Empty;

        public static string GetID(this Type type)
        {
            return
                (type?.GetCustomAttributes(typeof(IDAttribute), false)?.SingleOrDefault() as IDAttribute)?.ID
                ;
        }

        public static string[] GetAliases(this Type type)
        {
            return
                type?.GetCustomAttributes(typeof(AliasAttribute), false)?.SelectMany(aliasAttr => (aliasAttr as AliasAttribute)?.Aliases)?.Distinct()?.ToArray()
                ;
        }

        public static Note[] GetNotes(this Type type)
        {
            return
                type?.GetCustomAttributes(typeof(NoteAttribute), true)?.SelectMany(noteAttr => (noteAttr as NoteAttribute)?.Notes)?.ToArray()
                ;
        }

        public static Note[] GetNotes(this Type type, string noteID, bool isCaseInsensitive = false)
        {
            return
                type
                ?.GetNotes()
                ?.Where(note => string.Equals(note.ID, noteID, isCaseInsensitive ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture))
                .ToArray()
                ??
                new Note[0]
                ;

        }

        public static bool IsMatch(this Type type, string identifier)
        {
            if (type == null || string.IsNullOrWhiteSpace(identifier))
                return false;

            if (type.Name?.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase) ?? false)
                return true;

            if (string.Equals(identifier, type.GetID(), StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (type.GetAliases()?.Any(alias => string.Equals(identifier, alias, StringComparison.InvariantCultureIgnoreCase)) ?? false)
                return true;

            return false;
        }
    }
}
