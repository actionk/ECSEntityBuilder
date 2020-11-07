using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;
using Unity.Transforms;

namespace Plugins.ECSEntityBuilder.Steps
{
    public class SetParentStep : IEntityBuilderStep
    {
        private Entity m_parentEntity;

        public void SetValue(Entity parentEntity)
        {
            m_parentEntity = parentEntity;
        }

        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, Entity entity)
        {
            wrapper.AddComponentData(entity, new Parent {Value = m_parentEntity});
            wrapper.AddComponentData(entity, new LocalToParent());
        }
    }
}