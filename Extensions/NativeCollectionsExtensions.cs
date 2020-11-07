using System;
using Unity.Collections;

namespace Plugins.ECSEntityBuilder.Extensions
{
    public static class NativeCollectionsExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this NativeHashMap<TKey, TValue> hashMap, TKey key, TValue defaultValue)
            where TKey : struct, IEquatable<TKey>
            where TValue : struct
        {
            if (hashMap.TryGetValue(key, out TValue resultValue))
                return resultValue;

            return defaultValue;
        }
    }
}