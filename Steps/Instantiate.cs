using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Steps
{
    public class Instantiate : IEntityBuilderStep
    {
        private readonly Entity m_prefabEntity;

        public Instantiate(Entity prefabEntity)
        {
            m_prefabEntity = prefabEntity;
        }

        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, ref EntityBuilderData data)
        {
            data.entity = wrapper.Instantiate(m_prefabEntity);
        }
    }
}