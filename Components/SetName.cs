using Unity.Collections;
using Unity.Entities;

namespace Plugins.Shared.ECSEntityBuilder.Components
{
    public struct SetName : IComponentData
    {
        public FixedString64 Value;
    }
}