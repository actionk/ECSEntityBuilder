using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.InstantiationStrategy
{
    public class CreateFromPrefabStrategy : IEntityCreationStrategy
    {
        private readonly Entity m_prefabEntity;

        public CreateFromPrefabStrategy(Entity prefabEntity)
        {
            m_prefabEntity = prefabEntity;
        }

        public Entity Create(EntityManagerWrapper wrapper, EntityVariableMap variables)
        {
            return wrapper.Instantiate(m_prefabEntity);
        }
    }
}