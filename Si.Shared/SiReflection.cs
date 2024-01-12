using NTDLS.Semaphore;
using System.Collections.Generic;
using System.Reflection;

namespace Si.Shared
{
    public static class SiReflection
    {
        static readonly PessimisticSemaphore<Dictionary<string, Type>> _typeCache = new();
        static readonly PessimisticSemaphore<Dictionary<string, PropertyInfo>> _staticPropertyCache = new();
        static readonly PessimisticSemaphore<Dictionary<Type, List<Type>>> _subClassesOfCache = new();

        public static IEnumerable<Type> GetSubClassesOf<T>()
        {
            var cached = _subClassesOfCache.Use(o =>
            {
                o.TryGetValue(typeof(T), out var cached);
                return cached;
            });

            if (cached != null)
            {
                return cached;
            }

            List<Type> allTypes = [];

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                allTypes.AddRange(assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(T))));
            }

            _subClassesOfCache.Use(o => o.TryAdd(typeof(T), allTypes));

            return allTypes;
        }

        public static string? GetStaticPropertyValue(string typeName, string propertyName)
        {
            string key = $"[{typeName}].[{propertyName}]";

            var cached = _staticPropertyCache.Use(o =>
            {
                if (o.TryGetValue(key, out var cachedPropertyInfo))
                {
                    return cachedPropertyInfo.GetValue(null) as string;
                }
                return null;
            });

            if (cached != null)
            {
                return cached;
            }

            var type = GetTypeByName(typeName) ?? throw new Exception("Type not found.");

            var propertyInfo = type.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Static);
            if (propertyInfo != null)
            {
                _staticPropertyCache.Use(o => o.TryAdd(key, propertyInfo));
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
            var type = GetTypeByName(typeName) ?? throw new Exception("Type not found.");
            return (T?)Activator.CreateInstance(type, constructorArgs);
        }

        public static T? CreateInstanceFromTypeName<T>(string typeName)
        {
            var type = GetTypeByName(typeName) ?? throw new Exception("Type not found.");
            return (T?)Activator.CreateInstance(type);
        }

        public static bool DoesTypeExist(string typeName)
        {
            return GetTypeByName(typeName) != null;
        }

        public static Type? GetTypeByName(string typeName)
        {
            var cached = _typeCache.Use(o =>
            {
                o.TryGetValue(typeName, out var cachedType);
                return cachedType;
            });

            if (cached != null)
            {
                return cached;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
                if (type != null)
                {
                    _typeCache.Use(o => o.TryAdd(typeName, type));
                    return type;
                }
            }

            return null;
        }

        /// <summary>
        // Caches all types that inherit from T;
        /// </summary>
        public static void BuildReflectionCacheOfType<T>()
        {
            foreach (var item in GetSubClassesOf<T>())
            {
                _ = SiReflection.GetTypeByName(item.Name);
            }
        }

        public static T? CreateInstanceOf<T>(object[] constructorArgs)
        {
            return (T?)Activator.CreateInstance(typeof(T), constructorArgs);
        }
    }
}
