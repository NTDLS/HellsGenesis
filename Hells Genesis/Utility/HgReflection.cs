using System;
using System.Linq;
using System.Reflection;

namespace HG.Utility
{
    internal static class HgReflection
    {
        public static string GetStaticPropertyValue(string typeName, string propertyName)
        {
            var type = Assembly.GetExecutingAssembly().GetTypes().First(t => t.Name == typeName);
            var propertyInfo = type.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Static);
            return propertyInfo.GetValue(null) as string;
        }

        public static T CreateInstanceOf<T>(string typeName, object[] constructorArgs)
        {
            var type = Assembly.GetExecutingAssembly().GetTypes().First(t => t.Name == typeName);
            return (dynamic)Activator.CreateInstance(type, constructorArgs);
        }

        public static T CreateInstanceOf<T>(string typeName)
        {
            var type = Assembly.GetExecutingAssembly().GetTypes().First(t => t.Name == typeName);
            return (dynamic)Activator.CreateInstance(type);
        }
    }
}
