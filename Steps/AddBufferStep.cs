using System.Collections.Generic;
using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Steps
{
    public class AddBufferStep<T> : IEntityBuilderGenericStep<T> where T : struct, IBufferElementData
    {
        private readonly LinkedList<T> m_elements = new LinkedList<T>();

        public void Add(T element)
        {
            m_elements.AddLast(element);
        }

        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, Entity entity)
        {
            var buffer = wrapper.AddBuffer<T>(entity);
            foreach (var item in m_elements)
                buffer.Add(item);
        }
    }
}