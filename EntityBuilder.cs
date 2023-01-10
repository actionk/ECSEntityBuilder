using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.ECSEntityBuilder.Archetypes;
using Plugins.ECSEntityBuilder.InstantiationStrategy;
using Plugins.ECSEntityBuilder.Steps;
using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Plugins.ECSEntityBuilder
{
    public abstract class EntityBuilder
    {
        public delegate void OnPreBuildDelegate();

        public delegate void OnPostBuildDelegate(EntityWrapper entityWrapper);

        protected bool isBuilt;
        protected IEntityCreationStrategy creationStrategy = CreateEmptyStrategy.DEFAULT;
        protected EntityVariableMap variables;
        protected readonly LinkedList<IEntityBuilderStep> steps = new LinkedList<IEntityBuilderStep>();

        public event OnPreBuildDelegate OnPreBuildHandler;
        public event OnPostBuildDelegate OnPostBuildHandler;

        ~EntityBuilder()
        {
            if (!isBuilt)
            {
                throw new Exception($"EntityBuilder {GetType()} was never built");
            }
        }

        public bool HasStep<T, TGeneric>() where T : IEntityBuilderGenericStep<TGeneric>
        {
            return steps.FirstOrDefault(x => x is IEntityBuilderGenericStep<TGeneric>) != null;
        }

        public EntityBuilder AddStep(IEntityBuilderStep step)
        {
            steps.AddLast(step);
            return this;
        }

        public EntityBuilder SetVariable<T>(T variable) where T : class, IEntityVariable
        {
            if (variables == null)
                variables = new EntityVariableMap();

            variables.Set(variable);
            return this;
        }

        protected EntityBuilder CreateEmpty()
        {
            creationStrategy = new CreateEmptyStrategy();
            return this;
        }

        public EntityBuilder SetCreationStrategy(IEntityCreationStrategy strategy)
        {
            creationStrategy = strategy;
            return this;
        }

        protected EntityBuilder CreateFromArchetype<T>() where T : IArchetypeDescriptor
        {
            creationStrategy = new CreateFromArchetypeStrategy<T>();
            return this;
        }

        protected EntityBuilder CreateFromPrefab(Entity prefabEntity)
        {
            creationStrategy = new CreateFromPrefabStrategy(prefabEntity);
            return this;
        }

        public EntityBuilder AddComponent<T>() where T : unmanaged, IComponentData
        {
            GetOrCreateGenericStep<AddComponentStep<T>, T>();
            return this;
        }

        public EntityBuilder AddComponentData<T>(T component) where T : unmanaged, IComponentData
        {
            GetOrCreateGenericStep<AddComponentDataStep<T>, T>().SetValue(component);
            return this;
        }

        public EntityBuilder AddComponentObject<T>(T componentObject) where T : Component
        {
            GetOrCreateGenericStep<AddComponentObjectStep<T>, T>().SetValue(componentObject);
            return this;
        }

        public EntityBuilder SetComponentData<T>(T component) where T : unmanaged, IComponentData
        {
            GetOrCreateStep<SetComponentDataStep<T>>().SetValue(component);
            return this;
        }

        public EntityBuilder SetName(string name)
        {
            GetOrCreateStep<SetNameStep>().SetValue(name);
            return this;
        }

        public EntityBuilder AddSharedComponentData<T>(T component) where T : unmanaged, ISharedComponentData
        {
            GetOrCreateGenericStep<AddSharedComponentDataStep<T>, T>().SetValue(component);
            return this;
        }

        public T GetOrCreateGenericStep<T, TGenericValue>() where T : IEntityBuilderGenericStep<TGenericValue>, new()
        {
            var singletonStep = steps.FirstOrDefault(x => x is T);
            if (singletonStep != null)
                return (T) singletonStep;

            var createdSingletonStep = new T();
            steps.AddLast(createdSingletonStep);
            return createdSingletonStep;
        }

        public T GetOrCreateStep<T>() where T : IEntityBuilderStep, new()
        {
            var singletonStep = steps.FirstOrDefault(x => x.GetType() == typeof(T));
            if (singletonStep != null)
                return (T) singletonStep;

            var createdSingletonStep = new T();
            steps.AddLast(createdSingletonStep);
            return createdSingletonStep;
        }

        public EntityBuilder AddBuffer<T>(params T[] elements) where T : unmanaged, IBufferElementData
        {
            var step = GetOrCreateGenericStep<AddBufferStep<T>, T>();
            foreach (var element in elements)
                step.Add(element);
            return this;
        }

        public EntityBuilder AddBuffer<T>() where T : unmanaged, IBufferElementData
        {
            GetOrCreateGenericStep<AddBufferStep<T>, T>();
            return this;
        }

        public EntityBuilder AddElementToBuffer<T>(T element) where T : unmanaged, IBufferElementData
        {
            GetOrCreateGenericStep<AddBufferStep<T>, T>().Add(element);
            return this;
        }

        public EntityBuilder AddElementsToBuffer<T>(params T[] elements) where T : unmanaged, IBufferElementData
        {
            foreach (var element in elements)
                GetOrCreateGenericStep<AddBufferStep<T>, T>().Add(element);

            return this;
        }

        public EntityBuilder AddElementsToBuffer<T>(IEnumerable<T> elements) where T : unmanaged, IBufferElementData
        {
            foreach (var element in elements)
                GetOrCreateGenericStep<AddBufferStep<T>, T>().Add(element);

            return this;
        }

        public EntityBuilder SetParent(Entity entity)
        {
            GetOrCreateStep<SetParentStep>().SetValue(entity);
            return this;
        }

        public Entity Build()
        {
            return Build(EntityManagerWrapper.Default);
        }

        public Entity Build(EntityManager entityManager)
        {
            return Build(new EntityManagerWrapper(entityManager));
        }

        public Entity Build(EntityCommandBuffer entityCommandBuffer)
        {
            return Build(EntityManagerWrapper.FromCommandBuffer(entityCommandBuffer));
        }

        public Entity Build(EntityCommandBuffer.ParallelWriter entityCommandBuffer, int threadId)
        {
            return Build(EntityManagerWrapper.FromJobCommandBuffer(entityCommandBuffer, threadId));
        }

        public virtual Entity Build(EntityManagerWrapper wrapper)
        {
            isBuilt = true;
            var entity = creationStrategy.Create(wrapper, variables);

            OnPreBuildHandler?.Invoke();

            OnPreBuild(wrapper);

            foreach (var step in steps)
                step.Process(wrapper, variables, entity);

            OnPostBuildHandler?.Invoke(EntityWrapper.Wrap(entity, wrapper));

            return entity;
        }

        protected virtual void OnPreBuild(EntityManagerWrapper wrapper)
        {
        }

        public EntityWrapper BuildAndWrap(EntityManagerWrapper wrapper)
        {
            return new EntityWrapper(Build(wrapper), wrapper, variables);
        }

        public EntityWrapper BuildAndWrap()
        {
            return BuildAndWrap(EntityManagerWrapper.Default);
        }
    }
}