using Unity.Collections;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Components
{
    public struct SetName : IComponentData
    {
        public FixedString512Bytes value;
    }
}