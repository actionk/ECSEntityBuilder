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

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this DynamicBuffer<TValue> buffer, Func<TValue, TKey> mapping)
            where TValue : struct, IBufferElementData
        {
            var dictionary = new Dictionary<TKey, TValue>();
            foreach (var entry in buffer)
                dictionary.Add(mapping.Invoke(entry), entry);

            return dictionary;
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

        public static void ForEach<T>(this DynamicBuffer<T> buffer, Action<T> mappingFunction) where T : struct, IBufferElementData
        {
            for (var i = 0; i < buffer.Length; i++)
                mappingFunction.Invoke(buffer[i]);
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

        public delegate void UpdateDelegate<T>(ref T data);

        public static void UpdateAt<T>(this DynamicBuffer<T> buffer, int index, UpdateDelegate<T> action) where T : struct, IBufferElementData
        {
            var element = buffer[index];
            action(ref element);
            buffer[index] = element;
        }

        public static void UpdateIf<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate, UpdateDelegate<T> action) where T : struct, IBufferElementData
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                T element = buffer[i];
                if (predicate(element))
                {
                    UpdateAt(buffer, i, action);
                }
            }
        }

        public static int FindFirstIndex<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate) where T : struct, IBufferElementData
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (predicate(buffer[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public static bool FindFirstOrDefault<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate, out T result) where T : struct, IBufferElementData
        {
            foreach (var element in buffer)
            {
                if (predicate(element))
                {
                    result = element;
                    return true;
                }
            }

            result = default;
            return false;
        }

        public static void AddRange<T>(this DynamicBuffer<T> buffer, IEnumerable<T> values) where T : struct, IBufferElementData
        {
            foreach (var value in values)
                buffer.Add(value);
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