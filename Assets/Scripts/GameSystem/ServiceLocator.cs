using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public static void Register<T>(T service) => _services[typeof(T)] = service;

        public static T Get<T>()
        {
            if (_services == null)
                return default;

            if (_services.TryGetValue(typeof(T), out var service))
                return (T)service;

            return default;
        }
    }
}