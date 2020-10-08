using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Components
{
    public struct ManagedGameObject : IComponentData
    {
        public int instanceId;
    }
}