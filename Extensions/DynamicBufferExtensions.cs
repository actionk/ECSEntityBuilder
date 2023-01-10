﻿using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Extensions
{
    public static class DynamicBufferExtensions
    {
        public static T[] ToArray<T>(this DynamicBuffer<T> buffer) where T : unmanaged, IBufferElementData
        {
            var array = new T[buffer.Length];
            for (var i = 0; i < array.Length; i++)
                array[i] = buffer[i];
            return array;
        }

        public static HashSet<T> ToHashSet<T>(this DynamicBuffer<T> buffer) where T : unmanaged, IBufferElementData
        {
            var set = new HashSet<T>();
            for (var i = 0; i < buffer.Length; i++)
                set.Add(buffer[i]);
            return set;
        }

        public static HashSet<TOutput> ToHashSet<TInput, TOutput>(this DynamicBuffer<TInput> buffer, Func<TInput, TOutput> mappingFunction) where TInput : unmanaged, IBufferElementData
        {
            var set = new HashSet<TOutput>();
            for (var i = 0; i < buffer.Length; i++)
                set.Add(mappingFunction(buffer[i]));
            return set;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this DynamicBuffer<TValue> buffer, Func<TValue, TKey> mapping)
            where TValue : unmanaged, IBufferElementData
        {
            var dictionary = new Dictionary<TKey, TValue>();
            foreach (var entry in buffer)
                dictionary.Add(mapping.Invoke(entry), entry);

            return dictionary;
        }

        public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(this DynamicBuffer<T> buffer, Func<T, KeyValuePair<TKey, TValue>> mapping)
            where T : unmanaged, IBufferElementData
        {
            var dictionary = new Dictionary<TKey, TValue>();
            foreach (var entry in buffer)
            {
                var keyValuePair = mapping.Invoke(entry);
                dictionary.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return dictionary;
        }

        public static T2[] ToArray<T, T2>(this DynamicBuffer<T> buffer, Func<T, T2> mappingFunction) where T : unmanaged, IBufferElementData
        {
            var array = new T2[buffer.Length];
            for (var i = 0; i < array.Length; i++)
                array[i] = mappingFunction.Invoke(buffer[i]);

            return array;
        }

        public static void ForEach<T>(this DynamicBuffer<T> buffer, Action<T> mappingFunction) where T : unmanaged, IBufferElementData
        {
            for (var i = 0; i < buffer.Length; i++)
                mappingFunction.Invoke(buffer[i]);
        }

        public static int IndexOf<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate) where T : unmanaged, IBufferElementData
        {
            for (var i = 0; i < buffer.Length; i++)
            {
                if (predicate.Invoke(buffer[i]))
                    return i;
            }

            return -1;
        }
        
        public static bool None<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate) where T : unmanaged, IBufferElementData
            => !buffer.HasAny(predicate);

        public static bool HasAny<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate) where T : unmanaged, IBufferElementData
        {
            for (var i = 0; i < buffer.Length; i++)
            {
                if (predicate.Invoke(buffer[i]))
                    return true;
            }

            return false;
        }

        public static bool Contains<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate) where T : unmanaged, IBufferElementData
        {
            return buffer.IndexOf(predicate) != -1;
        }

        public static void AddOrReplace<T>(this DynamicBuffer<T> buffer, T element, Predicate<T> predicate) where T : unmanaged, IBufferElementData
        {
            var indexOfExistingElement = buffer.IndexOf(predicate);
            if (indexOfExistingElement >= 0)
                buffer[indexOfExistingElement] = element;
            else
                buffer.Add(element);
        }

        public delegate void UpdateDelegate<T>(ref T data);
        public delegate T CreateDelegate<T>();

        public static void UpdateAll<T>(this DynamicBuffer<T> buffer, UpdateDelegate<T> action) where T : unmanaged, IBufferElementData
        {
            for (int index = 0; index < buffer.Length; index++)
            {
                UpdateAt(buffer, index, action);
            }
        }

        public static void UpdateAt<T>(this DynamicBuffer<T> buffer, int index, UpdateDelegate<T> action) where T : unmanaged, IBufferElementData
        {
            var element = buffer[index];
            action(ref element);
            buffer[index] = element;
        }

        public static void UpdateIf<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate, UpdateDelegate<T> action) where T : unmanaged, IBufferElementData
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

        public static bool UpdateFirstIf<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate, UpdateDelegate<T> action) where T : unmanaged, IBufferElementData
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                T element = buffer[i];
                if (predicate(element))
                {
                    UpdateAt(buffer, i, action);
                    return true;
                }
            }

            return false;
        }

        public static bool UpdateFirstOrCreateIf<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate, UpdateDelegate<T> updateAction, CreateDelegate<T> createAction) where T : unmanaged, IBufferElementData
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                T element = buffer[i];
                if (predicate(element))
                {
                    UpdateAt(buffer, i, updateAction);
                    return true;
                }
            }

            buffer.Add(createAction());
            return false;
        }

        public static int FindFirstIndex<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate) where T : unmanaged, IBufferElementData
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
        
        public static void RemoveAll<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate) where T : unmanaged, IBufferElementData
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (predicate(buffer[i]))
                {
                    buffer.RemoveAt(i);
                    i--;
                }
            }
        }

        public static bool TryGetValue<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate, out T result) where T : unmanaged, IBufferElementData
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

        public static T GetValueOrDefault<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate) where T : unmanaged, IBufferElementData
        {
            foreach (var element in buffer)
            {
                if (predicate(element))
                    return element;
            }

            return default;
        }

        public static bool TryGetValueWithIndex<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate, out T result, out int index) where T : unmanaged, IBufferElementData
        {
            for(var i = 0; i < buffer.Length; i++)
            {
                var element = buffer[i];
                if (predicate(element))
                {
                    result = element;
                    index = i;
                    return true;
                }
            }

            result = default;
            index = default;
            return false;
        }

        public static void AddRange<T>(this DynamicBuffer<T> buffer, IEnumerable<T> values) where T : unmanaged, IBufferElementData
        {
            foreach (var value in values)
                buffer.Add(value);
        }

        public static bool RemoveFirst<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate) where T : unmanaged, IBufferElementData
        {
            var indexOfExistingElement = buffer.IndexOf(predicate);
            if (indexOfExistingElement >= 0)
                buffer.RemoveAt(indexOfExistingElement);

            return indexOfExistingElement >= 0;
        }

        public static bool ReplaceFirst<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate, T replaceWith) where T : unmanaged, IBufferElementData
        {
            var indexOfExistingElement = buffer.IndexOf(predicate);
            if (indexOfExistingElement >= 0)
            {
                buffer[indexOfExistingElement] = replaceWith;
                return true;
            }

            return false;
        }
    }
}