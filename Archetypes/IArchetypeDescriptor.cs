using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Archetypes
{
    public interface IArchetypeDescriptor
    {
        ComponentType[] Components { get; }
    }
}