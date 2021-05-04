using System;
using Plugins.ECSEntityBuilder.Archetypes;
using Plugins.ECSEntityBuilder.Components;
using Plugins.ECSEntityBuilder.Extensions;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Plugins.ECSEntityBuilder
{
    public class EntityManagerWrapper
    {
        public static EntityManagerWrapper Default => new EntityManagerWrapper(World.DefaultGameObjectInjectionWorld.EntityManager);
        public static EntityManagerWrapper Mock => new EntityManagerWrapper();

        public enum EntityManagerType
        {
            MOCK,
            ENTITY_MANAGER,
            ENTITY_COMMAND_BUFFER,
            ENTITY_COMMAND_BUFFER_CONCURRENT,
            ENTITY_MANAGER_AND_COMMAND_BUFFER
        }

        public EntityManagerType Type { get; }
        public EntityManager EntityManager { get; }
        public EntityCommandBuffer.ParallelWriter EntityCommandBufferConcurrent { get; }
        public EntityCommandBuffer EntityCommandBuffer { get; }

        public int EntityCommandBufferJobIndex { get; }

        public override string ToString()
        {
            return $"{nameof(Type)}: {Type}";
        }

        public static EntityManagerWrapper FromManager(EntityManager dstManager)
        {
            return new EntityManagerWrapper(dstManager);
        }

        public static EntityManagerWrapper From(EntityManager dstManager)
        {
            return new EntityManagerWrapper(dstManager);
        }

        public static EntityManagerWrapper FromManagerAndBuffer(EntityManager dstManager, EntityCommandBuffer entityCommandBuffer)
        {
            return new EntityManagerWrapper(dstManager, entityCommandBuffer);
        }

        public static EntityManagerWrapper From(EntityManager dstManager, EntityCommandBuffer entityCommandBuffer)
        {
            return new EntityManagerWrapper(dstManager, entityCommandBuffer);
        }

        public static EntityManagerWrapper From(ComponentSystem componentSystem)
        {
            return new EntityManagerWrapper(componentSystem.EntityManager, componentSystem.PostUpdateCommands);
        }

        private EntityManagerWrapper()
        {
            Type = EntityManagerType.MOCK;
        }

        public EntityManagerWrapper(EntityManager entityManager)
        {
            EntityManager = entityManager;
            Type = EntityManagerType.ENTITY_MANAGER;
        }

        public EntityManagerWrapper(EntityManager entityManager, EntityCommandBuffer entityCommandBuffer)
        {
            EntityManager = entityManager;
            EntityCommandBuffer = entityCommandBuffer;
            Type = EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER;
        }

        public EntityManagerWrapper(EntityCommandBuffer.ParallelWriter entityCommandBufferConcurrent, int jobIndex)
        {
            EntityCommandBufferConcurrent = entityCommandBufferConcurrent;
            EntityCommandBufferJobIndex = jobIndex;
            Type = EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT;
        }

        public EntityManagerWrapper(EntityCommandBuffer entityCommandBuffer)
        {
            EntityCommandBuffer = entityCommandBuffer;
            Type = EntityManagerType.ENTITY_COMMAND_BUFFER;
        }

        public static EntityManagerWrapper FromJobCommandBuffer(EntityCommandBuffer.ParallelWriter commandBuffer, int jobIndex)
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
                case EntityManagerType.MOCK:
                    return Entity.Null;
                case EntityManagerType.ENTITY_MANAGER:
                    return EntityManager.CreateEntity();
                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
                    return EntityCommandBuffer.CreateEntity();
                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    return EntityCommandBufferConcurrent.CreateEntity(EntityCommandBufferJobIndex);
            }

            throw new NotImplementedException();
        }

        public void SetName(Entity entity, string name)
        {
#if UNITY_EDITOR
            switch (Type)
            {
                case EntityManagerType.MOCK:
                    return;
                case EntityManagerType.ENTITY_MANAGER:
                    EntityManager.AddComponentData(entity, new SetName {value = new FixedString64(name)});
                    return;
                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
                    EntityCommandBuffer.AddComponent(entity, new SetName {value = new FixedString64(name)});
                    return;
                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    EntityCommandBufferConcurrent.AddComponent(EntityCommandBufferJobIndex, entity, new SetName {value = new FixedString64(name)});
                    return;
            }

            throw new NotImplementedException();
#endif
        }

        public Entity CreateEntity(EntityArchetype entityArchetype)
        {
            switch (Type)
            {
                case EntityManagerType.MOCK:
                    return Entity.Null;
                case EntityManagerType.ENTITY_MANAGER:
                    return EntityManager.CreateEntity(entityArchetype);
                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
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
                case EntityManagerType.MOCK:
                    return;
                case EntityManagerType.ENTITY_MANAGER:
                    EntityManager.AddComponentData(entity, component);
                    return;
                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
                    EntityCommandBuffer.AddComponent(entity, component);
                    return;
                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    EntityCommandBufferConcurrent.AddComponent(EntityCommandBufferJobIndex, entity, component);
                    return;
            }

            throw new NotImplementedException();
        }

        public void AddComponent<T>(Entity entity) where T : struct, IComponentData
        {
            switch (Type)
            {
                case EntityManagerType.MOCK:
                    return;
                case EntityManagerType.ENTITY_MANAGER:
                    EntityManager.AddComponent<T>(entity);
                    return;
                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
                    EntityCommandBuffer.AddComponent<T>(entity);
                    return;
                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    EntityCommandBufferConcurrent.AddComponent<T>(EntityCommandBufferJobIndex, entity);
                    return;
            }

            throw new NotImplementedException();
        }

        public void SetComponentData<T>(Entity entity, T component) where T : struct, IComponentData
        {
            switch (Type)
            {
                case EntityManagerType.MOCK:
                    return;
                case EntityManagerType.ENTITY_MANAGER:
                    EntityManager.SetComponentData(entity, component);
                    return;
                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
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
                case EntityManagerType.MOCK:
                    return;
                case EntityManagerType.ENTITY_MANAGER:
                    EntityManager.AddSharedComponentData(entity, component);
                    return;
                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
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
                case EntityManagerType.MOCK:
                    return default;
                case EntityManagerType.ENTITY_MANAGER:
                    return EntityManager.AddBuffer<T>(entity);
                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
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
                case EntityManagerType.MOCK:
                    return default;
                case EntityManagerType.ENTITY_MANAGER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
                    return EntityManager.GetBuffer<T>(entity);
            }

            throw new NotImplementedException();
        }

        public DynamicBuffer<T> GetOrCreateBuffer<T>(Entity entity) where T : struct, IBufferElementData
        {
            switch (Type)
            {
                case EntityManagerType.MOCK:
                    return default;
                
                case EntityManagerType.ENTITY_MANAGER:
                    return EntityManager.HasComponent<T>(entity) ? EntityManager.GetBuffer<T>(entity) : EntityManager.AddBuffer<T>(entity);
                
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
                    return EntityManager.HasComponent<T>(entity) ? EntityManager.GetBuffer<T>(entity) : EntityCommandBuffer.AddBuffer<T>(entity);
            }

            throw new NotImplementedException();
        }

        public bool HasComponent<T>(Entity entity) where T : struct
        {
            switch (Type)
            {
                case EntityManagerType.MOCK:
                    return false;
                case EntityManagerType.ENTITY_MANAGER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
                    return EntityManager.HasComponent<T>(entity);
            }

            throw new NotImplementedException();
        }

        public EntityArchetype CreateArchetype(ComponentType[] components)
        {
            switch (Type)
            {
                case EntityManagerType.MOCK:
                    return default;
                case EntityManagerType.ENTITY_MANAGER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
                    return EntityManager.CreateArchetype(components);
            }

            throw new NotImplementedException();
        }

        public void Destroy(Entity entity)
        {
            switch (Type)
            {
                case EntityManagerType.MOCK:
                    return;

                case EntityManagerType.ENTITY_MANAGER:
                    EntityManager.DestroyEntity(entity);
                    return;

                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
                    EntityCommandBuffer.DestroyEntity(entity);
                    return;

                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    EntityCommandBufferConcurrent.DestroyEntity(EntityCommandBufferJobIndex, entity);
                    return;
            }
        }

        public Entity Instantiate(Entity prefabEntity)
        {
            switch (Type)
            {
                case EntityManagerType.MOCK:
                    return Entity.Null;
                case EntityManagerType.ENTITY_MANAGER:
                    return EntityManager.Instantiate(prefabEntity);
                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
                    return EntityCommandBuffer.Instantiate(prefabEntity);
                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    return EntityCommandBufferConcurrent.Instantiate(EntityCommandBufferJobIndex, prefabEntity);
            }

            throw new NotImplementedException();
        }

        public T GetComponentData<T>(Entity entity) where T : struct, IComponentData
        {
            switch (Type)
            {
                case EntityManagerType.MOCK:
                    return default;
                case EntityManagerType.ENTITY_MANAGER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
                    return EntityManager.GetComponentData<T>(entity);
            }

            throw new NotImplementedException();
        }

        public void AddComponentObject<T>(Entity entity, T componentObject) where T : Component
        {
            switch (Type)
            {
                case EntityManagerType.MOCK:
                    return;
                case EntityManagerType.ENTITY_MANAGER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
                    EntityManager.AddComponentObject(entity, componentObject);
                    return;
            }

            throw new NotImplementedException("Adding component object only possible using EntityManager, not buffer");
        }

        public void AppendToBuffer<T>(Entity entity, T bufferElementData) where T : struct, IBufferElementData
        {
            switch (Type)
            {
                case EntityManagerType.MOCK:
                    return;

                case EntityManagerType.ENTITY_MANAGER:
                    var buffer = EntityManager.HasComponent<T>(entity) ? EntityManager.GetBuffer<T>(entity) : EntityManager.AddBuffer<T>(entity);
                    buffer.Add(bufferElementData);
                    break;

                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
                    EntityCommandBuffer.AppendToBuffer(entity, bufferElementData);
                    return;

                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    EntityCommandBufferConcurrent.AppendToBuffer(EntityCommandBufferJobIndex, entity, bufferElementData);
                    return;
            }

            throw new NotImplementedException("Adding component object only possible using EntityManager, not buffer");
        }

        public bool RemoveComponent<T>(Entity entity) where T : struct, IComponentData
        {
            switch (Type)
            {
                case EntityManagerType.MOCK:
                    return false;

                case EntityManagerType.ENTITY_MANAGER:
                    return EntityManager.RemoveComponent<T>(entity);

                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
                    EntityCommandBuffer.RemoveComponent<T>(entity);
                    return true;

                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    EntityCommandBufferConcurrent.RemoveComponent<T>(EntityCommandBufferJobIndex, entity);
                    return true;
            }

            throw new NotImplementedException();
        }

        public bool RemoveComponent(Entity entity, ComponentType componentType)
        {
            switch (Type)
            {
                case EntityManagerType.MOCK:
                    return false;

                case EntityManagerType.ENTITY_MANAGER:
                    return EntityManager.RemoveComponent(entity, componentType);

                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
                    EntityCommandBuffer.RemoveComponent(entity, componentType);
                    return true;

                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    EntityCommandBufferConcurrent.RemoveComponent(EntityCommandBufferJobIndex, entity, componentType);
                    return true;
            }

            throw new NotImplementedException();
        }

        public Entity CreateEntityFromArchetype<T>() where T : IArchetypeDescriptor
        {
            var archetype = EntityArchetypeManager.Instance.GetOrCreateArchetype<T>();

            switch (Type)
            {
                case EntityManagerType.MOCK:
                    return Entity.Null;
                case EntityManagerType.ENTITY_MANAGER:
                    return EntityManager.CreateEntity(archetype);
                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
                    return EntityCommandBuffer.CreateEntity(archetype);
                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    return EntityCommandBufferConcurrent.CreateEntity(EntityCommandBufferJobIndex, archetype);
            }

            throw new NotImplementedException();
        }

        public EntityWrapper Wrap(Entity entity)
        {
            return EntityWrapper.Wrap(entity, this);
        }

        public DynamicBuffer<T> ReplaceElementsInBuffer<T>(Entity entity, params T[] elements) where T : struct, IBufferElementData
        {
            var buffer = AddBuffer<T>(entity);
            buffer.Clear();
            buffer.AddRange(elements);
            return buffer;
        }

        public void ToggleComponent<T>(Entity entity, bool value) where T: struct,IComponentData
        {
            if (value)
                AddComponent<T>(entity);
            else
                RemoveComponent<T>(entity);
        }

        public bool RemoveBuffer<T>(Entity entity) where T : struct, IBufferElementData
        {
            switch (Type)
            {
                case EntityManagerType.MOCK:
                    return false;

                case EntityManagerType.ENTITY_MANAGER:
                    return EntityManager.RemoveComponent<T>(entity);

                case EntityManagerType.ENTITY_COMMAND_BUFFER:
                case EntityManagerType.ENTITY_MANAGER_AND_COMMAND_BUFFER:
                    EntityCommandBuffer.RemoveComponent<T>(entity);
                    return true;

                case EntityManagerType.ENTITY_COMMAND_BUFFER_CONCURRENT:
                    EntityCommandBufferConcurrent.RemoveComponent<T>(EntityCommandBufferJobIndex, entity);
                    return true;
            }

            throw new NotImplementedException();
        }
    }
}