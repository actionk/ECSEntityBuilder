using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Steps
{
    public class SetComponentData<T> : IEntityBuilderStep where T : struct, IComponentData
    {
        private readonly T m_componentData;

        public SetComponentData(T componentData)
        {
            m_componentData = componentData;
        }

        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, ref EntityBuilderData data)
        {
            wrapper.SetComponentData(data.entity, m_componentData);
        }
    }
}