using System.Reflection;

namespace Si.Shared
{
    public static class SiReflection
    {
        public static IEnumerable<Type> GetSubClassesOf<T>()
        {
            List<Type> allTypes = new();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                allTypes.AddRange(assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(T))));
            }

            return allTypes;
        }

        public static string? GetStaticPropertyValue(string typeName, string propertyName)
        {
            var type = GetTypeByName(typeName);
            var propertyInfo = type.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Static);
            return propertyInfo?.GetValue(null) as string;
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
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
                if (type != null)
                {
                    return type;
                }
            }
            throw new Exception("Type not found.");
        }

        public static T? CreateInstanceOf<T>(object[] constructorArgs)
        {
            return (T?)Activator.CreateInstance(typeof(T), constructorArgs);

            /*
            ConstructorInfo constructor = type.GetConstructor(constructorArgs.Select(obj => obj.GetType()).ToArray());
            if (constructor == null)
            {
                throw new InvalidOperationException($"No matching constructor found for type {type.Name}");
            }

            T instance = (T)constructor.Invoke(constructorArgs);

            return instance;
            */
        }
    }
}
