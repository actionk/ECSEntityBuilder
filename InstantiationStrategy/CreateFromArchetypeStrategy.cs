using Plugins.Shared.ECSEntityBuilder.Archetypes;
using Plugins.Shared.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Plugins.Shared.ECSEntityBuilder.InstantiationStrategy
{
    public class CreateFromArchetypeStrategy<T> : IEntityCreationStrategy where T : IArchetypeDescriptor
    {
        public Entity Create(EntityManagerWrapper wrapper, EntityVariableMap variables)
        {
            return wrapper.CreateEntity(EntityArchetypeManager.Instance.GetOrCreateArchetype<T>());
        }
    }
}