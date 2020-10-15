using Plugins.ECSEntityBuilder.Components;
using Plugins.ECSEntityBuilder.Managers;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class ManagedGameObjectSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<ManagedGameObject>()
                .ForEach((Entity entity, ref ManagedGameObject managedGameObject) =>
                {
                    var gameObject = GameObjectEntityManager.Instance[managedGameObject.instanceId];
                    EntityManager.AddComponentObject(entity, gameObject.transform);
                    gameObject.SetActive(true);
                    PostUpdateCommands.RemoveComponent<ManagedGameObject>(entity);
                });
        }
    }
}