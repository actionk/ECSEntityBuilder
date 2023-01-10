using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Steps
{
    public class AddSharedComponentDataStep<T> : IEntityBuilderGenericStep<T> where T : unmanaged, ISharedComponentData
    {
        private T m_componentData;

        public void SetValue(T componentData)
        {
            m_componentData = componentData;
        }

        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, Entity entity)
        {
            wrapper.AddSharedComponentData(entity, m_componentData);
        }
    }
}