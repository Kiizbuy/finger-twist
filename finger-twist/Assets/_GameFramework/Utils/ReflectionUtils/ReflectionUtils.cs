using GameFramework.Extension;
using GameFramework.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFramework.Utils.Reflection
{
    public static class ReflectionUtils
    {
        private static Dictionary<Type, List<Type>> _allInterfaceTypeImplementations = null;
        private static readonly string _mainAssemblyName = "Assembly-CSharp";

#if UNITY_EDITOR

        static ReflectionUtils()
        {
            CacheStrategyInterfaceTypes();
        }

        private static void CacheStrategyInterfaceTypes()
        {
            Debug.Log("Start Caching interface types");

            _allInterfaceTypeImplementations = new Dictionary<Type, List<Type>>();

            var cSharpAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == _mainAssemblyName);

            if (cSharpAssembly == null)
            {
                Debug.LogError("Main Assembly doesn't exsist on this project");
                return;
            }

            var strategies = cSharpAssembly.GetTypes()
                            .Where(x => x.IsInterface && x.GetInterfaces().Contains(typeof(IStrategyContainer)))
                            .ToList();
             
            foreach (var interfaceKeyType in strategies)
                _allInterfaceTypeImplementations.Add(interfaceKeyType, cSharpAssembly.GetAllDerivedTypes(interfaceKeyType));

            Debug.Log("Caching interface types has been complete");

        }
#endif

        public static List<Type> TryGetStrategyImplementationsFromType(Type concreteImplementationType)
        {
            if (_allInterfaceTypeImplementations.TryGetValue(concreteImplementationType, out var strategyList))
                return strategyList;

            Debug.LogError($"{concreteImplementationType.Name} doesn't exsist");
            return null;
        }
    }
}