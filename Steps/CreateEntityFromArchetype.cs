using Plugins.ECSEntityBuilder.Archetypes;
using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Steps
{
    public class CreateEntityFromArchetype<T> : IEntityBuilderStep where T : IArchetypeDescriptor
    {
        private static EntityArchetype GetEntityArchetype(EntityManagerWrapper wrapper)
        {
            return EntityArchetypeManager.Instance.GetOrCreateArchetype<T>(wrapper);
        }

        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, ref EntityBuilderData data)
        {
            data.entity = wrapper.CreateEntity(GetEntityArchetype(wrapper));
            wrapper.SetName(data.entity, typeof(T).Name + " " + data.entity.Index);
        }
    }
}