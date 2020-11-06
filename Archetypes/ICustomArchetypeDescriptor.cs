using Unity.Entities;

namespace Plugins.Shared.ECSEntityBuilder.Archetypes
{
    public interface ICustomArchetypeDescriptor : IArchetypeDescriptor
    {
        ComponentType[] CustomComponents { get; }
    }
}