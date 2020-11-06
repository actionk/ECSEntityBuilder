using Plugins.Shared.ECSEntityBuilder.Archetypes;
using Plugins.Shared.ECSEntityBuilder.Variables;
using Plugins.Shared.ECSEntityBuilder.Worlds;
using Unity.Entities;

namespace Plugins.Shared.ECSEntityBuilder.InstantiationStrategy
{
    public class CreateFromArchetypeStrategy<T> : IEntityCreationStrategy where T : IArchetypeDescriptor
    {
        private readonly WorldType m_worldType;

        public CreateFromArchetypeStrategy(WorldType worldType)
        {
            m_worldType = worldType;
        }

        public Entity Create(EntityManagerWrapper wrapper, EntityVariableMap variables)
        {
            return wrapper.CreateEntity(EntityArchetypeManager.Instance.GetOrCreateArchetype<T>(m_worldType));
        }
    }
}