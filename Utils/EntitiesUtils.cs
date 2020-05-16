using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Utils
{
    public static class EntitiesUtils
    {
        public static void AddSystemComponentIfNotExists<TSelector, TComponent>(this EntityQueryBuilder entityQueryBuilder, EntityCommandBuffer entityCommandBuffer) where TComponent : struct, IComponentData
        {
            entityQueryBuilder
                .WithAll<TSelector>()
                .WithNone<TComponent>()
                .ForEach(entityCommandBuffer.AddComponent<TComponent>);
        }

        public static void RemoveComponentIfNotExists<TSelector, TComponent>(this EntityQueryBuilder entityQueryBuilder, EntityCommandBuffer entityCommandBuffer) where TComponent : struct, IComponentData
        {
            entityQueryBuilder
                .WithNone<TSelector>()
                .WithAll<TComponent>()
                .ForEach(entityCommandBuffer.RemoveComponent<TComponent>);
        }
    }
}