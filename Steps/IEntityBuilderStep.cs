using Plugins.ECSEntityBuilder.Variables;

namespace Plugins.ECSEntityBuilder.Steps
{
    public interface IEntityBuilderStep
    {
        void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, ref EntityBuilderData data);
    }
}