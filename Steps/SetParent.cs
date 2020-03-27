using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;
using Unity.Transforms;

namespace Plugins.ECSEntityBuilder.Steps
{
    public class SetParent : IEntityBuilderStep
    {
        private readonly Entity m_parentEntity;

        public SetParent(Entity parentEntity)
        {
            m_parentEntity = parentEntity;
        }

        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, ref EntityBuilderData data)
        {
            wrapper.AddComponentData(data.entity, new Parent {Value = m_parentEntity});
            wrapper.AddComponentData(data.entity, new LocalToParent());
        }
    }
}