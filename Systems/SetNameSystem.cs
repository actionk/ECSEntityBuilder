using Plugins.ECSEntityBuilder.Components;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class SetNameSystem : ComponentSystem
    {
        protected override void OnUpdateImpl()
        {
            Entities
                .WithNone<HasName>()
                .ForEach((Entity entity, ref SetName setName) =>
                {
#if UNITY_EDITOR
                    EntityManager.SetName(entity, setName.value.ToString());
#endif
                    PostUpdateCommands.AddComponent<HasName>(entity);
                })
                .WithoutBurst().Run();
        }
    }
}