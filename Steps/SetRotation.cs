using Plugins.ECSEntityBuilder.Variables;
using Unity.Mathematics;
using Unity.Transforms;

namespace Plugins.ECSEntityBuilder.Steps
{
    public class SetRotation : IEntityBuilderStep
    {
        private quaternion m_rotation;

        public SetRotation(quaternion rotation)
        {
            m_rotation = rotation;
        }

        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, ref EntityBuilderData data)
        {
            wrapper.SetComponentData(data.entity, new Rotation {Value = m_rotation});
        }
    }
}