using Unity.Entities;

namespace Plugins.Shared.ECSEntityBuilder.Archetypes
{
    public interface IArchetypeDescriptor
    {
        ComponentType[] Components { get; }
    }
}