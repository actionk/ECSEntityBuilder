using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;
using UnityEngine;

namespace Plugins.ECSEntityBuilder.Steps
{
    public class AddComponentObjectStep<T> : IEntityBuilderGenericStep<T> where T : Component
    {
        private T m_componentObject;

        public void SetValue(T componentData)
        {
            m_componentObject = componentData;
        }

        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, Entity entity)
        {
            wrapper.AddComponentObject(entity, m_componentObject);
        }
    }
}