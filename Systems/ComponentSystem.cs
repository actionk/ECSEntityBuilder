using Unity.Collections;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Systems
{
    public abstract partial class ComponentSystem : SystemBase
    {
        private bool m_entityCommandBufferInitialized;
        private EntityCommandBuffer m_entityCommandBuffer;
        
        public EntityCommandBuffer PostUpdateCommands
        {
            get
            {
                if (m_entityCommandBufferInitialized)
                    return m_entityCommandBuffer;

                m_entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);
                m_entityCommandBufferInitialized = true;
                return m_entityCommandBuffer;
            }
        }

        protected override void OnUpdate()
        {
            OnUpdateImpl();

            if (m_entityCommandBufferInitialized)
            {
                m_entityCommandBuffer.Playback(EntityManager);
                m_entityCommandBuffer.Dispose();
                m_entityCommandBufferInitialized = false;
            }
        }

        protected abstract void OnUpdateImpl();
    }
}