using Core.Entities.Variables;
using Plugins.ECSEntityBuilder;
using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Core.Entities.EntityBuilderSteps
{
    public class AddComponentData<T> : IEntityBuilderStep where T : struct, IComponentData
    {
        private readonly T m_componentData;

        public AddComponentData(T componentData)
        {
            m_componentData = componentData;
        }

        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, ref EntityBuilderData data)
        {
            wrapper.AddComponentData(data.entity, m_componentData);
        }
    }
}