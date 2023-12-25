using System.Reflection;

namespace Si.Shared
{
    public static class SiReflection
    {
        static readonly Dictionary<string, Type> _typeCache = new();
        static readonly Dictionary<string, PropertyInfo> _staticPropertyCache = new();
        static readonly Dictionary<Type, List<Type>> _subClassesOfCache = new();

        public static IEnumerable<Type> GetSubClassesOf<T>()
        {
            if (_subClassesOfCache.TryGetValue(typeof(T), out var cached))
            {
                return cached;
            }

            List<Type> allTypes = new();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                allTypes.AddRange(assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(T))));
            }

            _subClassesOfCache.TryAdd(typeof(T), allTypes);

            return allTypes;
        }

        public static string? GetStaticPropertyValue(string typeName, string propertyName)
        {
            string key = $"[{typeName}].[{propertyName}]";

            if (_staticPropertyCache.TryGetValue(key, out var cachedPropertyInfo))
            {
                return cachedPropertyInfo.GetValue(null) as string;
            }

            var type = GetTypeByName(typeName);
            var propertyInfo = type.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Static);
            if (propertyInfo != null)
            {
                _staticPropertyCache.TryAdd(key, propertyInfo);

                return propertyInfo.GetValue(null) as string;
            }

            throw new Exception("Static property not found.");
        }

        public static T? CreateInstanceFromType<T>(Type type, object[] constructorArgs)
        {
            return (T?)Activator.CreateInstance(type, constructorArgs);
        }

        public static T? CreateInstanceFromType<T>(Type type)
        {
            return (T?)Activator.CreateInstance(type);
        }

        public static T? CreateInstanceFromTypeName<T>(string typeName, object[] constructorArgs)
        {
            var type = Assembly.GetExecutingAssembly().GetTypes().First(t => t.Name == typeName);
            return (T?)Activator.CreateInstance(type, constructorArgs);
        }

        public static T? CreateInstanceFromTypeName<T>(string typeName)
        {
            var type = Assembly.GetExecutingAssembly().GetTypes().First(t => t.Name == typeName);
            return (T?)Activator.CreateInstance(type);
        }

        public static Type GetTypeByName(string typeName)
        {
            if (_typeCache.TryGetValue(typeName, out var cachedType))
            {
                return cachedType;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
                if (type != null)
                {
                    _typeCache.TryAdd(typeName, type);

                    return type;
                }
            }
            throw new Exception("Type not found.");
        }

        public static T? CreateInstanceOf<T>(object[] constructorArgs)
        {
            return (T?)Activator.CreateInstance(typeof(T), constructorArgs);
        }
    }
}
