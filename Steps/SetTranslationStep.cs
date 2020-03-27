using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Plugins.ECSEntityBuilder.Steps
{
    public class SetTranslationStep : IEntityBuilderStep
    {
        private float3 m_position;

        public void SetValue(float3 position)
        {
            m_position = position;
        }

        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, Entity entity)
        {
            wrapper.SetComponentData(entity, new Translation {Value = m_position});
        }
    }
}