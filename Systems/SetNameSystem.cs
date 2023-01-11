using Plugins.ECSEntityBuilder.Components;
using Unity.Entities;
using Unity.Mathematics;

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
                    var nameValue = setName.value.Value;
                    EntityManager.SetName(entity, nameValue.Substring(0, math.min(60, nameValue.Length)));
#endif
                    PostUpdateCommands.AddComponent<HasName>(entity);
                })
                .WithoutBurst().Run();
        }
    }
}