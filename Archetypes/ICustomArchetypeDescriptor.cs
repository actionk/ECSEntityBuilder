using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Archetypes
{
    public interface ICustomArchetypeDescriptor : IArchetypeDescriptor
    {
        ComponentType[] CustomComponents { get; }
    }
}