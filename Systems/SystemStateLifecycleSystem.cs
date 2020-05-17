using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Plugins.ECSEntityBuilder.Systems
{
    public abstract class SystemStateLifecycleSystem<TSelector, TSystemState> : JobComponentSystem
        where TSelector : struct, IComponentData
        where TSystemState : struct, ISystemStateComponentData
    {
        private EntityQuery m_newEntities;
        private EntityQuery m_destroyedEntities;
        private EntityCommandBufferSystem m_ecbSource;

        protected override void OnCreate()
        {
            m_newEntities = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] {ComponentType.ReadOnly<TSelector>()},
                None = new[] {ComponentType.ReadWrite<TSystemState>()}
            });

            m_destroyedEntities = GetEntityQuery(new EntityQueryDesc
            {
                None = new[] {ComponentType.ReadWrite<TSelector>()},
                All = new[] {ComponentType.ReadOnly<TSystemState>()}
            });

            m_ecbSource = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

#pragma warning disable 618
        struct NewEntityJob : IJobForEachWithEntity<TSelector>
        {
            public EntityCommandBuffer.Concurrent ConcurrentECB;

            public void Execute(Entity entity, int jobIndex, [ReadOnly] ref TSelector component)
            {
                // Add an ISystemStateComponentData instance
                ConcurrentECB.AddComponent<TSystemState>(jobIndex, entity);
            }
        }

        struct CleanupEntityJob : IJobForEachWithEntity<TSystemState>
        {
            public EntityCommandBuffer.Concurrent ConcurrentECB;

            public void Execute(Entity entity, int jobIndex, [ReadOnly] ref TSystemState state)
            {
                // This system is responsible for removing any ISystemStateComponentData instances it adds
                // Otherwise, the entity is never truly destroyed.
                ConcurrentECB.RemoveComponent<TSystemState>(jobIndex, entity);
            }
        }
#pragma warning restore 618

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var newEntityJob = new NewEntityJob
            {
                ConcurrentECB = m_ecbSource.CreateCommandBuffer().ToConcurrent()
            };
            var newJobHandle = newEntityJob.ScheduleSingle(m_newEntities, inputDependencies);
            m_ecbSource.AddJobHandleForProducer(newJobHandle);

            var cleanupEntityJob = new CleanupEntityJob
            {
                ConcurrentECB = m_ecbSource.CreateCommandBuffer().ToConcurrent()
            };
            var cleanupJobHandle = cleanupEntityJob.ScheduleSingle(m_destroyedEntities, newJobHandle);
            m_ecbSource.AddJobHandleForProducer(cleanupJobHandle);

            return cleanupJobHandle;
        }
    }
}