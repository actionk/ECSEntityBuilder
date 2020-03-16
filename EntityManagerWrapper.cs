using System;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder
{
    public class EntityManagerWrapper
    {
        public static EntityManagerWrapper Default => new EntityManagerWrapper(World.DefaultGameObjectInjectionWorld.EntityManager);

        public enum EntityManagerType
        {
            ENTITY_MANAGER,
            ENTITY_COMMAND_BUFFER,
            ENTITY_COMMAND_BUFFER_CONCURRENT
        }

        public EntityManagerType Type { get; }
        public EntityManager EntityManager { get; }
        public EntityCommandBuffer.Concurrent EntityCommandBufferConcurrent { get; }
        public EntityCommandBuffer EntityCommandBuffer { get; }
        public int EntityCommandBufferJobIndex { get; }

        public EntityManagerWrapper(EntityManager entityManager)
        {
            EntityManager = entityManager;
            Type = EntityManagerType.ENTITY_MANAGER;
        }

        public EntityManagerWrapper(EntityCommandBuffer.Concurrent entityCommandBufferConcurrent, int jobIndex)
        {
            EntityCommandBufferConcurrent = entityCommandBufferConcurrent;
            EntityCommandBufferJobIndex = jobIndex;
            Type = EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT;
        }

        public EntityManagerWrapper(EntityCommandBuffer entityCommandBufferConcurrent)
        {
            EntityCommandBuffer = entityCommandBufferConcurrent;
            Type = EntityManagerType.ENTITY_COMMAND_BUFFER;
        }

        public static EntityManagerWrapper FromJobCommandBuffer(EntityCommandBuffer.Concurrent commandBuffer, int jobIndex)
        {
            return new EntityManagerWrapper(commandBuffer, jobIndex);
        }

        public static EntityManagerWrapper FromCommandBuffer(EntityCommandBuffer commandBuffer)
        {
            return new EntityManagerWrapper(commandBuffer);
        }

        public Entity CreateEntity()
        {
            switch (Type)
            {
                case EntityManagerType.ENTITY_MANAGER:
                    return EntityManager.CreateEntity();
                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                    return EntityCommandBuffer.CreateEntity();
                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    return EntityCommandBufferConcurrent.CreateEntity(EntityCommandBufferJobIndex);
            }

            throw new NotImplementedException();
        }

        public void SetName(Entity entity, string name)
        {
            switch (Type)
            {
                case EntityManagerType.ENTITY_MANAGER:
                    EntityManager.SetName(entity, name);
                    return;
                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    return;
            }

            throw new NotImplementedException();
        }

        public Entity CreateEntity(EntityArchetype entityArchetype)
        {
            switch (Type)
            {
                case EntityManagerType.ENTITY_MANAGER:
                    return EntityManager.CreateEntity(entityArchetype);
                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                    return EntityCommandBuffer.CreateEntity(entityArchetype);
                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    return EntityCommandBufferConcurrent.CreateEntity(EntityCommandBufferJobIndex, entityArchetype);
            }

            throw new NotImplementedException();
        }

        public void AddComponentData<T>(Entity entity, T component) where T : struct, IComponentData
        {
            switch (Type)
            {
                case EntityManagerType.ENTITY_MANAGER:
                    EntityManager.AddComponentData(entity, component);
                    return;
                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                    EntityCommandBuffer.AddComponent(entity, component);
                    return;
                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    EntityCommandBufferConcurrent.AddComponent(EntityCommandBufferJobIndex, entity, component);
                    return;
            }

            throw new NotImplementedException();
        }

        public void SetComponentData<T>(Entity entity, T component) where T : struct, IComponentData
        {
            switch (Type)
            {
                case EntityManagerType.ENTITY_MANAGER:
                    EntityManager.SetComponentData(entity, component);
                    return;
                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                    EntityCommandBuffer.SetComponent(entity, component);
                    return;
                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    EntityCommandBufferConcurrent.SetComponent(EntityCommandBufferJobIndex, entity, component);
                    return;
            }

            throw new NotImplementedException();
        }

        public void AddSharedComponentData<T>(Entity entity, T component) where T : struct, ISharedComponentData
        {
            switch (Type)
            {
                case EntityManagerType.ENTITY_MANAGER:
                    EntityManager.AddSharedComponentData(entity, component);
                    return;
                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                    EntityCommandBuffer.AddSharedComponent(entity, component);
                    return;
                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    EntityCommandBufferConcurrent.AddSharedComponent(EntityCommandBufferJobIndex, entity, component);
                    return;
            }

            throw new NotImplementedException();
        }

        public DynamicBuffer<T> AddBuffer<T>(Entity entity) where T : struct, IBufferElementData
        {
            switch (Type)
            {
                case EntityManagerType.ENTITY_MANAGER:
                    return EntityManager.AddBuffer<T>(entity);
                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                    return EntityCommandBuffer.AddBuffer<T>(entity);
                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    return EntityCommandBufferConcurrent.AddBuffer<T>(EntityCommandBufferJobIndex, entity);
            }

            throw new NotImplementedException();
        }

        public DynamicBuffer<T> GetBuffer<T>(Entity entity) where T : struct, IBufferElementData
        {
            switch (Type)
            {
                case EntityManagerType.ENTITY_MANAGER:
                    return EntityManager.GetBuffer<T>(entity);
            }

            throw new NotImplementedException();
        }

        public EntityArchetype CreateArchetype(ComponentType[] components)
        {
            switch (Type)
            {
                case EntityManagerType.ENTITY_MANAGER:
                    return EntityManager.CreateArchetype(components);
            }

            throw new NotImplementedException();
        }

        public void Destroy(Entity entity)
        {
            switch (Type)
            {
                case EntityManagerType.ENTITY_MANAGER:
                    EntityManager.DestroyEntity(entity);
                    return;

                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                    EntityCommandBuffer.DestroyEntity(entity);
                    return;

                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    EntityCommandBufferConcurrent.DestroyEntity(EntityCommandBufferJobIndex, entity);
                    return;
            }
        }
    }
}