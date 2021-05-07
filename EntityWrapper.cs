using System;
using Plugins.ECSEntityBuilder.Archetypes;
using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;
using UnityEngine;

namespace Plugins.ECSEntityBuilder
{
    public class EntityWrapper
    {
        public Entity Entity { get; private set; }
        public EntityManagerWrapper EntityManagerWrapper { get; }
        public EntityVariableMap Variables { get; }

        public static EntityWrapper CreateEntity()
        {
            var entityManagerWrapper = EntityManagerWrapper.Default;
            return new EntityWrapper(entityManagerWrapper.CreateEntity(), entityManagerWrapper);
        }

        public static EntityWrapper CreateEntity(EntityManager entityManager)
        {
            var entityManagerWrapper = EntityManagerWrapper.FromManager(entityManager);
            return new EntityWrapper(entityManagerWrapper.CreateEntity(), entityManagerWrapper);
        }

        public static EntityWrapper CreateEntity<T>(EntityManager entityManager) where T : IArchetypeDescriptor
        {
            var entityManagerWrapper = EntityManagerWrapper.FromManager(entityManager);
            return new EntityWrapper(entityManagerWrapper.CreateEntityFromArchetype<T>(), entityManagerWrapper);
        }

        public static EntityWrapper CreateEntity(EntityCommandBuffer entityCommandBuffer)
        {
            var entityManagerWrapper = EntityManagerWrapper.FromCommandBuffer(entityCommandBuffer);
            return new EntityWrapper(entityManagerWrapper.CreateEntity(), entityManagerWrapper);
        }

        public static EntityWrapper CreateEntity<T>(EntityCommandBuffer entityCommandBuffer) where T : IArchetypeDescriptor
        {
            var entityManagerWrapper = EntityManagerWrapper.FromCommandBuffer(entityCommandBuffer);
            return new EntityWrapper(entityManagerWrapper.CreateEntityFromArchetype<T>(), entityManagerWrapper);
        }

        public static EntityWrapper CreateEntity<T>(EntityManagerWrapper entityManagerWrapper) where T : IArchetypeDescriptor
        {
            return new EntityWrapper(entityManagerWrapper.CreateEntityFromArchetype<T>(), entityManagerWrapper);
        }

        public static EntityWrapper CreateEntityFromArchetype(EntityArchetype archetype, EntityManagerWrapper entityManagerWrapper)
        {
            return new EntityWrapper(entityManagerWrapper.CreateEntityFromArchetype(archetype), entityManagerWrapper);
        }

        public static EntityWrapper CreateEntity(int threadId, EntityCommandBuffer.ParallelWriter entityCommandBuffer)
        {
            var entityManagerWrapper = EntityManagerWrapper.FromJobCommandBuffer(entityCommandBuffer, threadId);
            return new EntityWrapper(entityManagerWrapper.CreateEntity(), entityManagerWrapper);
        }

        public static EntityWrapper CreateEntity<T>(int threadId, EntityCommandBuffer.ParallelWriter entityCommandBuffer) where T : IArchetypeDescriptor
        {
            var entityManagerWrapper = EntityManagerWrapper.FromJobCommandBuffer(entityCommandBuffer, threadId);
            return new EntityWrapper(entityManagerWrapper.CreateEntityFromArchetype<T>(), entityManagerWrapper);
        }

        public static EntityWrapper CreateEntity(EntityManagerWrapper entityManagerWrapper)
        {
            return new EntityWrapper(entityManagerWrapper.CreateEntity(), entityManagerWrapper);
        }

        public static EntityWrapper Instantiate(Entity prefab, EntityManager entityManager)
        {
            var entityManagerWrapper = EntityManagerWrapper.FromManager(entityManager);
            return new EntityWrapper(entityManagerWrapper.Instantiate(prefab), entityManagerWrapper);
        }

        public static EntityWrapper Instantiate(Entity prefab, EntityCommandBuffer entityCommandBuffer)
        {
            var entityManagerWrapper = EntityManagerWrapper.FromCommandBuffer(entityCommandBuffer);
            return new EntityWrapper(entityManagerWrapper.Instantiate(prefab), entityManagerWrapper);
        }

        public static EntityWrapper Instantiate(Entity prefab, int threadId, EntityCommandBuffer.ParallelWriter entityCommandBuffer)
        {
            var entityManagerWrapper = EntityManagerWrapper.FromJobCommandBuffer(entityCommandBuffer, threadId);
            return new EntityWrapper(entityManagerWrapper.Instantiate(prefab), entityManagerWrapper);
        }

        public static EntityWrapper Wrap(Entity entity, ComponentSystem componentSystem)
        {
            return new EntityWrapper(entity, EntityManagerWrapper.From(componentSystem));
        }

        public static EntityWrapper Wrap(Entity entity, EntityManagerWrapper entityManagerWrapper)
        {
            return new EntityWrapper(entity, entityManagerWrapper);
        }

        public static EntityWrapper Wrap(Entity entity, EntityManager entityManager)
        {
            return new EntityWrapper(entity, EntityManagerWrapper.FromManager(entityManager));
        }

        public static EntityWrapper Wrap(Entity entity, EntityCommandBuffer entityCommandBuffer)
        {
            return new EntityWrapper(entity, EntityManagerWrapper.FromCommandBuffer(entityCommandBuffer));
        }

        public static EntityWrapper Wrap(Entity entity, EntityCommandBuffer.ParallelWriter entityCommandBuffer, int threadId)
        {
            return new EntityWrapper(entity, EntityManagerWrapper.FromJobCommandBuffer(entityCommandBuffer, threadId));
        }

        public EntityWrapper(Entity entity, EntityManagerWrapper entityManagerWrapper, EntityVariableMap variables)
        {
            Entity = entity;
            EntityManagerWrapper = entityManagerWrapper;
            Variables = variables;
        }

        public EntityWrapper(Entity entity, EntityManagerWrapper entityManagerWrapper)
        {
            Entity = entity;
            EntityManagerWrapper = entityManagerWrapper;
            Variables = new EntityVariableMap();
        }

        public EntityWrapper(Entity entity)
        {
            Entity = entity;
            EntityManagerWrapper = EntityManagerWrapper.Default;
            Variables = new EntityVariableMap();
        }

        public EntityWrapper UsingWrapper(EntityManagerWrapper wrapper, Action<EntityWrapper> callback)
        {
            var newEntityWrapper = new EntityWrapper(Entity, wrapper, Variables);
            callback.Invoke(newEntityWrapper);
            return this;
        }

        public EntityWrapper SetVariable<T>(T variable) where T : class, IEntityVariable
        {
            Variables.Set(variable);
            return this;
        }

        public T GetVariable<T>() where T : class, IEntityVariable
        {
            return Variables.Get<T>();
        }

        public EntityWrapper SetName(string name)
        {
            EntityManagerWrapper.SetName(Entity, name);
            return this;
        }

        public EntityWrapper AddComponentData<T>(T component) where T : struct, IComponentData
        {
            EntityManagerWrapper.AddComponentData(Entity, component);
            return this;
        }

        public EntityWrapper AddComponent<T>() where T : struct, IComponentData
        {
            EntityManagerWrapper.AddComponent<T>(Entity);
            return this;
        }

        public EntityWrapper SetComponentData<T>(T component) where T : struct, IComponentData
        {
            EntityManagerWrapper.SetComponentData(Entity, component);
            return this;
        }

        public EntityWrapper AddSharedComponentData<T>(T component) where T : struct, ISharedComponentData
        {
            EntityManagerWrapper.AddSharedComponentData(Entity, component);
            return this;
        }

        public DynamicBuffer<T> AddBufferAndReturn<T>() where T : struct, IBufferElementData
        {
            return EntityManagerWrapper.AddBuffer<T>(Entity);
        }

        public EntityWrapper AppendToBuffer<T>(T elementData) where T : struct, IBufferElementData
        {
            EntityManagerWrapper.AppendToBuffer<T>(Entity, elementData);
            return this;
        }

        public EntityWrapper AddBuffer<T>() where T : struct, IBufferElementData
        {
            EntityManagerWrapper.AddBuffer<T>(Entity);
            return this;
        }

        public EntityWrapper AddBuffer<T>(int repeat) where T : struct, IBufferElementData
        {
            var buffer = EntityManagerWrapper.AddBuffer<T>(Entity);
            for (var i = 0; i < repeat; i++)
                buffer.Add(new T());

            return this;
        }

        public EntityWrapper AddElementsToBuffer<T>(params T[] elements) where T : struct, IBufferElementData
        {
            var buffer = EntityManagerWrapper.AddBuffer<T>(Entity);
            foreach (var element in elements)
                buffer.Add(element);
            return this;
        }

        public EntityWrapper ReplaceElementsInBuffer<T>(params T[] elements) where T : struct, IBufferElementData
        {
            if (Entity.Index < 0)
                return AddElementsToBuffer(elements);

            var buffer = EntityManagerWrapper.GetOrCreateBuffer<T>(Entity);
            buffer.Clear();
            foreach (var element in elements)
                buffer.Add(element);
            return this;
        }

        public DynamicBuffer<T> GetBuffer<T>() where T : struct, IBufferElementData
        {
            return EntityManagerWrapper.GetBuffer<T>(Entity);
        }

        public DynamicBuffer<T> GetOrCreateBuffer<T>() where T : struct, IBufferElementData
        {
            if (EntityManagerWrapper.HasComponent<T>(Entity))
                return EntityManagerWrapper.GetBuffer<T>(Entity);

            return AddBufferAndReturn<T>();
        }

        public EntityWrapper Destroy()
        {
            EntityManagerWrapper.Destroy(Entity);
            Entity = Entity.Null;
            return this;
        }

        public T GetComponentData<T>() where T : struct, IComponentData
        {
            return EntityManagerWrapper.GetComponentData<T>(Entity);
        }

        public T GetComponentDataOrDefault<T>() where T : struct, IComponentData
        {
            return EntityManagerWrapper.HasComponent<T>(Entity) ? EntityManagerWrapper.GetComponentData<T>(Entity) : default(T);
        }

        public bool HasComponent<T>() where T : struct
        {
            return EntityManagerWrapper.HasComponent<T>(Entity);
        }

        public bool RemoveComponent<T>() where T : struct, IComponentData
        {
            return EntityManagerWrapper.RemoveComponent<T>(Entity);
        }

        public void AddComponentObject(Component componentObject)
        {
            EntityManagerWrapper.AddComponentObject(Entity, componentObject);
        }

        public DynamicBuffer<T> AddOrCreateBuffer<T>() where T : struct, IBufferElementData
        {
            return EntityManagerWrapper.HasComponent<T>(Entity) ? EntityManagerWrapper.GetBuffer<T>(Entity) : EntityManagerWrapper.AddBuffer<T>(Entity);
        }
    }
}