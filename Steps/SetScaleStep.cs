using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;
using Unity.Transforms;

namespace Plugins.ECSEntityBuilder.Steps
{
    public class SetScaleStep : IEntityBuilderStep
    {
        private float m_scale;

        public void SetValue(float scale)
        {
            m_scale = scale;
        }

        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, Entity entity)
        {
            wrapper.AddComponentData(entity, new Scale {Value = m_scale});
        }
    }
}