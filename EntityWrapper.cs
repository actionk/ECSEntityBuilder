using System;
using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder
{
    public class EntityWrapper
    {
        public Entity Entity { get; private set; }
        public EntityManagerWrapper EntityManagerWrapper { get; }
        public EntityVariableMap Variables { get; }

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

        public void UsingWrapper(EntityManagerWrapper wrapper, Action<EntityWrapper> callback)
        {
            var newEntityWrapper = new EntityWrapper(Entity, wrapper, Variables);
            callback.Invoke(newEntityWrapper);
        }

        public void SetVariable<T>(T variable) where T : class, IEntityVariable
        {
            Variables.Set(variable);
        }

        public T GetVariable<T>() where T : class, IEntityVariable
        {
            return Variables.Get<T>();
        }

        public void SetName(string name)
        {
            EntityManagerWrapper.SetName(Entity, name);
        }

        public void AddComponentData<T>(T component) where T : struct, IComponentData
        {
            EntityManagerWrapper.AddComponentData(Entity, component);
        }

        public void SetComponentData<T>(T component) where T : struct, IComponentData
        {
            EntityManagerWrapper.SetComponentData(Entity, component);
        }

        public void AddSharedComponentData<T>(T component) where T : struct, ISharedComponentData
        {
            EntityManagerWrapper.AddSharedComponentData(Entity, component);
        }

        public DynamicBuffer<T> AddBuffer<T>() where T : struct, IBufferElementData
        {
            return EntityManagerWrapper.AddBuffer<T>(Entity);
        }

        public DynamicBuffer<T> AddBuffer<T>(params T[] elements) where T : struct, IBufferElementData
        {
            var buffer = EntityManagerWrapper.AddBuffer<T>(Entity);
            foreach (var element in elements)
                buffer.Add(element);
            return buffer;
        }

        public DynamicBuffer<T> AddElementToBuffer<T>(T element) where T : struct, IBufferElementData
        {
            var buffer = EntityManagerWrapper.AddBuffer<T>(Entity);
            buffer.Add(element);
            return buffer;
        }

        public DynamicBuffer<T> GetBuffer<T>() where T : struct, IBufferElementData
        {
            return EntityManagerWrapper.GetBuffer<T>(Entity);
        }

        public void Destroy()
        {
            EntityManagerWrapper.Destroy(Entity);
            Entity = Entity.Null;
        }
    }
}