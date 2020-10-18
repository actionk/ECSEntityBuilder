using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Extensions
{
    public static class DynamicBufferExtensions
    {
        public static T[] ToArray<T>(this DynamicBuffer<T> buffer) where T : struct,IBufferElementData
        {
            var array = new T[buffer.Length];
            for (var i = 0; i < array.Length; i++)
                array[i] = buffer[i];
            return array;
        }
    }
}