using Plugins.Shared.ECSEntityBuilder.Components;
using Unity.Entities;

namespace Plugins.Shared.ECSEntityBuilder.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class SetNameSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities
                .WithNone<HasName>()
                .ForEach((Entity entity, ref SetName setName) =>
                {
#if UNITY_EDITOR
                    EntityManager.SetName(entity, setName.Value.ToString());
#endif
                    PostUpdateCommands.AddComponent<HasName>(entity);
                });
        }
    }
}