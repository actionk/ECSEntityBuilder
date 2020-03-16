using Core.Entities.Variables;
using Plugins.ECSEntityBuilder;
using Plugins.ECSEntityBuilder.Variables;

namespace Core.Entities.EntityBuilderSteps
{
    public interface IEntityBuilderStep
    {
        void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, ref EntityBuilderData data);
    }
}