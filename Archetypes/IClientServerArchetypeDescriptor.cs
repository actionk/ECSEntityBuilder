using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Archetypes
{
    public interface IClientServerArchetypeDescriptor : IArchetypeDescriptor
    {
        ComponentType[] ClientOnlyComponents { get; }
        ComponentType[] ServerOnlyComponents { get; }
    }
}