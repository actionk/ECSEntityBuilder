using Plugins.ECSEntityBuilder.Variables;

namespace Plugins.ECSEntityBuilder.Steps
{
    public class CreateEntity : IEntityBuilderStep
    {
        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, ref EntityBuilderData data)
        {
            data.entity = wrapper.CreateEntity();

#if UNITY_EDITOR
            if (data.name != null)
                wrapper.SetName(data.entity, data.name);
#endif
        }
    }
}