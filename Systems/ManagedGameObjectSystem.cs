using Plugins.ECSEntityBuilder.Components;
using Plugins.ECSEntityBuilder.Managers;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class ManagedGameObjectSystem : ComponentSystem
    {
        private struct ManagedGameObjectSpawned : ISystemStateComponentData
        {
            public int instanceId;
        }

        protected override void OnUpdate()
        {
            Entities
                .WithAll<ManagedGameObject>()
                .WithNone<ManagedGameObjectSpawned>()
                .ForEach((Entity entity, ref ManagedGameObject managedGameObject) =>
                {
                    var gameObject = GameObjectEntityManager.Instance[managedGameObject.instanceId];
                    EntityManager.AddComponentObject(entity, gameObject.transform);
                    gameObject.SetActive(true);
                    PostUpdateCommands.AddComponent(entity, new ManagedGameObjectSpawned {instanceId = managedGameObject.instanceId});
                });

            Entities
                .WithAll<ManagedGameObjectSpawned>()
                .WithNone<ManagedGameObject>()
                .ForEach((Entity entity, ref ManagedGameObjectSpawned managedGameObjectSpawned) =>
                {
                    GameObjectEntityManager.Instance.Destroy(managedGameObjectSpawned.instanceId);
                    PostUpdateCommands.RemoveComponent<ManagedGameObjectSpawned>(entity);
                });
        }
    }
}