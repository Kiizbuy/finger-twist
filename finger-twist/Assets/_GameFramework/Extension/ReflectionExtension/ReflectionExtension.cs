using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GameFramework.Extension
{
    public static class ReflectionExtension
    {
        private static readonly string _mainAssemblyName = "Assembly-CSharp";

        public static IEnumerable<Type> GetInheritanceHierarchy(this Type type)
        {
            for (var current = type; current != null; current = current.BaseType)
                yield return current;
        }

        public static List<Type> GetAllDerivedTypes(this AppDomain appDomain, Type type)
        {
            var cSharpAssembly = appDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == _mainAssemblyName);

            if (cSharpAssembly == null)
            {
                Debug.LogError("Main Assembly doesn't exsist on this project");
                return null;
            }

            return cSharpAssembly.GetAllDerivedTypes(type);
        }

        public static List<Type> GetAllDerivedTypes(this Assembly appAssembly, Type type, string assemblyName = "Assembly-CSharp")
        {

            if (appAssembly.GetName().Name != assemblyName)
            {
                Debug.LogError($"Assembly with '{assemblyName}' name doesn't exsist on this project");
                return null;
            }

            return appAssembly.GetTypes()
                  .Where(x => type.IsAssignableFrom(x) && x != type && !x.IsAbstract && !typeof(MonoBehaviour).IsAssignableFrom(x))
                  .OrderBy(x => x.Name)
                  .ToList();
        }
    }
}

