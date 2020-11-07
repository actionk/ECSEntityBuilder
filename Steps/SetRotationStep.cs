using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Plugins.ECSEntityBuilder.Steps
{
    public class SetRotationStep : IEntityBuilderStep
    {
        private quaternion m_rotation;

        public void SetValue(quaternion rotation)
        {
            m_rotation = rotation;
        }

        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, Entity entity)
        {
            wrapper.SetComponentData(entity, new Rotation {Value = m_rotation});
        }
    }
}