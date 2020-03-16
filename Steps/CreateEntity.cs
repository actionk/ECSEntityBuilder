using Core.Entities.Variables;
using Plugins.ECSEntityBuilder;
using Plugins.ECSEntityBuilder.Variables;

namespace Core.Entities.EntityBuilderSteps
{
    public class CreateEntity : IEntityBuilderStep
    {
        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, ref EntityBuilderData data)
        {
            data.entity = wrapper.CreateEntity();
        }
    }
}