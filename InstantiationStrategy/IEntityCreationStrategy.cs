using Plugins.Shared.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Plugins.Shared.ECSEntityBuilder.InstantiationStrategy
{
    public interface IEntityCreationStrategy
    {
        Entity Create(EntityManagerWrapper wrapper, EntityVariableMap variables);
    }
}