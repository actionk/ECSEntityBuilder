using Unity.Mathematics;

namespace Plugins.Shared.ECSEntityBuilder.Extensions
{
    public static class MathematicsExtensions
    {
        public static float3 ConvertTo3D(this int2 value)
        {
            return new float3(value.x, 0, value.y);
        }

        public static int2 ConvertTo2D(this float3 value)
        {
            return new int2((int) value.x, (int) value.z);
        }
    }
}