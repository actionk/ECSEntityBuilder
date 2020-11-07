using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.InstantiationStrategy
{
    public interface IEntityCreationStrategy
    {
        Entity Create(EntityManagerWrapper wrapper, EntityVariableMap variables);
    }
}