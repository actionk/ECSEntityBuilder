using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Extensions
{
    public static class DynamicBufferExtensions
    {
        public static T[] ToArray<T>(this DynamicBuffer<T> buffer) where T : struct, IBufferElementData
        {
            var array = new T[buffer.Length];
            for (var i = 0; i < array.Length; i++)
                array[i] = buffer[i];
            return array;
        }

        public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(this DynamicBuffer<T> buffer, Func<T, KeyValuePair<TKey, TValue>> mapping)
            where T : struct, IBufferElementData
        {
            var dictionary = new Dictionary<TKey, TValue>();
            foreach (var entry in buffer)
            {
                var keyValuePair = mapping.Invoke(entry);
                dictionary.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return dictionary;
        }

        public static T2[] ToArray<T, T2>(this DynamicBuffer<T> buffer, Func<T, T2> mappingFunction) where T : struct, IBufferElementData
        {
            var array = new T2[buffer.Length];
            for (var i = 0; i < array.Length; i++)
                array[i] = mappingFunction.Invoke(buffer[i]);

            return array;
        }

        public static int IndexOf<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate) where T : struct, IBufferElementData
        {
            for (var i = 0; i < buffer.Length; i++)
            {
                if (predicate.Invoke(buffer[i]))
                    return i;
            }

            return -1;
        }

        public static void AddOrReplace<T>(this DynamicBuffer<T> buffer, T element, Predicate<T> predicate) where T : struct, IBufferElementData
        {
            var indexOfExistingElement = buffer.IndexOf(predicate);
            if (indexOfExistingElement >= 0)
                buffer[indexOfExistingElement] = element;
            else
                buffer.Add(element);
        }

        public static bool RemoveFirst<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate) where T : struct, IBufferElementData
        {
            var indexOfExistingElement = buffer.IndexOf(predicate);
            if (indexOfExistingElement >= 0)
                buffer.RemoveAt(indexOfExistingElement);

            return indexOfExistingElement >= 0;
        }
    }
}