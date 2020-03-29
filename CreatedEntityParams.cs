using Unity.Mathematics;

namespace Plugins.ECSEntityBuilder
{
    public class CreatedEntityParams
    {
        public float3 translation = float3.zero;
        public quaternion rotation = quaternion.identity;
        public float scale = 1.0f;
    }
}