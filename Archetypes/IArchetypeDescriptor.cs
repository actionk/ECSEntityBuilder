using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Archetypes
{
    public interface IArchetypeDescriptor
    {
        string Name { get; }
        ComponentType[] Components { get; }
    }
}