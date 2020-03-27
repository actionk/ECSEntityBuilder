using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Steps
{
    public interface IEntityBuilderStep
    {
        void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, Entity entity);
    }
}