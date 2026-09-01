using System;
using System.Collections.Concurrent;
using System.Linq;

namespace MauiApp1.BackEnd.Shared
{
    // Simple thread-safe in-memory cache for repository results.
    internal static class CacheManager
    {
        private static readonly ConcurrentDictionary<string, object> _cache = new();

        public static T GetOrAdd<T>(string key, Func<T> factory)
        {
            if (_cache.TryGetValue(key, out var existing) && existing is T t)
                return t;

            var value = factory();
            _cache[key] = value!;
            return value!;
        }

        public static void Remove(string key)
        {
            _cache.TryRemove(key, out _);
        }

        public static void RemovePrefix(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                return;

            var keys = _cache.Keys.Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var k in keys)
                _cache.TryRemove(k, out _);
        }

        public static void Clear() => _cache.Clear();
    }
}
