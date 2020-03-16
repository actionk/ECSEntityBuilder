using System;
using Core.Entities.Variables;
using Plugins.ECSEntityBuilder;
using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Core.Entities.EntityBuilderSteps
{
    public class AddBuffer<T> : IEntityBuilderStep where T : struct, IBufferElementData
    {
        private readonly T[] m_elements;
        private readonly Action<EntityManagerWrapper> m_callback;

        public AddBuffer(T[] elements)
        {
            m_elements = elements;
        }

        public AddBuffer()
        {
            m_elements = null;
        }

        public AddBuffer(Action<EntityManagerWrapper> callback)
        {
            m_callback = callback;
        }

        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, ref EntityBuilderData data)
        {
            if (m_callback != null)
            {
                m_callback?.Invoke(wrapper);
            }
            else
            {
                var buffer = wrapper.AddBuffer<T>(data.entity);
                if (m_elements != null)
                {
                    foreach (var element in m_elements)
                        buffer.Add(element);
                }
            }
        }
    }
}