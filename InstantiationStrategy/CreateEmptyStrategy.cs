using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.InstantiationStrategy
{
    public class CreateEmptyStrategy : IEntityCreationStrategy
    {
        public Entity Create(EntityManagerWrapper wrapper, EntityVariableMap variables)
        {
            return wrapper.CreateEntity();
        }

        public static readonly CreateEmptyStrategy DEFAULT = new CreateEmptyStrategy();
    }
}