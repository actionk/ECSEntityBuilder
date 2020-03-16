using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Steps
{
    public class AddSharedComponentData<T> : IEntityBuilderStep where T : struct, ISharedComponentData
    {
        private readonly T m_componentData;

        public AddSharedComponentData(T componentData)
        {
            m_componentData = componentData;
        }

        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, ref EntityBuilderData data)
        {
            wrapper.AddSharedComponentData(data.entity, m_componentData);
        }
    }
}