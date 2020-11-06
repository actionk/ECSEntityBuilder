using Unity.Entities;

namespace Plugins.Shared.ECSEntityBuilder.Archetypes
{
    public interface IClientServerArchetypeDescriptor : IArchetypeDescriptor
    {
        ComponentType[] ClientOnlyComponents { get; }
        ComponentType[] ServerOnlyComponents { get; }
    }
}