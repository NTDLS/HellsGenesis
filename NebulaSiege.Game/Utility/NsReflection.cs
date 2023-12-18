using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NebulaSiege.Game.Utility
{
    internal static class NsReflection
    {
        public static IEnumerable<Type> GetSubClassesOf<T>()
        {
            var allTypes = Assembly.GetExecutingAssembly().GetTypes();
            return allTypes.Where(type => type.IsSubclassOf(typeof(T)));
        }

        public static string GetStaticPropertyValue(string typeName, string propertyName)
        {
            var type = Assembly.GetExecutingAssembly().GetTypes().First(t => t.Name == typeName);
            var propertyInfo = type.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Static);
            return propertyInfo.GetValue(null) as string;
        }

        public static T CreateInstanceFromType<T>(Type type, object[] constructorArgs)
        {
            return (T)Activator.CreateInstance(type, constructorArgs);
        }

        public static T CreateInstanceFromType<T>(Type type)
        {
            return (T)Activator.CreateInstance(type);
        }

        public static T CreateInstanceFromTypeName<T>(string typeName, object[] constructorArgs)
        {
            var type = Assembly.GetExecutingAssembly().GetTypes().First(t => t.Name == typeName);
            return (T)Activator.CreateInstance(type, constructorArgs);
        }

        public static T CreateInstanceFromTypeName<T>(string typeName)
        {
            var type = Assembly.GetExecutingAssembly().GetTypes().First(t => t.Name == typeName);
            return (T)Activator.CreateInstance(type);
        }

        public static Type GetTypeByName(string typeName)
        {
            return Assembly.GetExecutingAssembly().GetTypes().First(t => t.Name == typeName);
        }


        public static T CreateInstanceOf<T>(object[] constructorArgs)
        {
            return (T)Activator.CreateInstance(typeof(T), constructorArgs);

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
