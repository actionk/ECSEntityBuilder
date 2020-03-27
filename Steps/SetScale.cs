using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;
using Unity.Transforms;

namespace Plugins.ECSEntityBuilder.Steps
{
    public class SetScale : IEntityBuilderStep
    {
        private readonly float m_scale;

        public SetScale(float scale)
        {
            m_scale = scale;
        }

        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, ref EntityBuilderData data)
        {
            if (wrapper.HasComponent<Scale>(data.entity))
                wrapper.SetComponentData(data.entity, new Scale {Value = m_scale});
            else
                wrapper.AddComponentData(data.entity, new Scale {Value = m_scale});
        }
    }
}