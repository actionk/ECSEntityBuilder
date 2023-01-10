using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Steps
{
    public class AddComponentStep<T> : IEntityBuilderGenericStep<T> where T : unmanaged, IComponentData
    {
        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, Entity entity)
        {
            wrapper.AddComponent<T>(entity);
        }
    }
}