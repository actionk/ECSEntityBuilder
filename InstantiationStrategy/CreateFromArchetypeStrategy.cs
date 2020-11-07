using Plugins.ECSEntityBuilder.Archetypes;
using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.InstantiationStrategy
{
    public class CreateFromArchetypeStrategy<T> : IEntityCreationStrategy where T : IArchetypeDescriptor
    {
        public Entity Create(EntityManagerWrapper wrapper, EntityVariableMap variables)
        {
            return wrapper.CreateEntity(EntityArchetypeManager.Instance.GetOrCreateArchetype<T>());
        }
    }
}