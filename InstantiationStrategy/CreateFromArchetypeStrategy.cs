using Plugins.ECSEntityBuilder.Archetypes;
using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.InstantiationStrategy
{
    public class CreateFromArchetypeStrategy<T> : IEntityCreationStrategy where T : IArchetypeDescriptor
    {
        private static EntityArchetype GetEntityArchetype(EntityManagerWrapper wrapper)
        {
            return EntityArchetypeManager.Instance.GetOrCreateArchetype<T>(wrapper);
        }

        public Entity Create(EntityManagerWrapper wrapper, EntityVariableMap variables)
        {
            return wrapper.CreateEntity(GetEntityArchetype(wrapper));
        }
    }
}