using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Steps
{
    public class SetComponentDataStep<T> : IEntityBuilderGenericStep<T> where T : struct, IComponentData
    {
        private T m_componentData;

        public void SetValue(T componentData)
        {
            m_componentData = componentData;
        }

        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, Entity entity)
        {
            wrapper.SetComponentData(entity, m_componentData);
        }
    }
}