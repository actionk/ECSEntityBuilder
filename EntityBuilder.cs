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
    public abstract class EntityBuilder<TChild>
    {
        protected abstract TChild Self { get; }

        protected bool m_built;
        protected IEntityCreationStrategy m_creationStrategy = CreateEmptyStrategy.DEFAULT;
        protected readonly EntityVariableMap m_variables = new EntityVariableMap();
        protected readonly LinkedList<IEntityBuilderStep> m_steps = new LinkedList<IEntityBuilderStep>();
        protected readonly LinkedList<Action<EntityWrapper>> m_postBuildActions = new LinkedList<Action<EntityWrapper>>();

        ~EntityBuilder()
        {
            if (!m_built)
            {
                throw new Exception($"EntityBuilder {Self} was never built");
            }
        }

        public bool HasStep<T, TGeneric>() where T : IEntityBuilderGenericStep<TGeneric>
        {
            return m_steps.FirstOrDefault(x => x is IEntityBuilderGenericStep<TGeneric>) != null;
        }

        public TChild AddStep(IEntityBuilderStep step)
        {
            m_steps.AddLast(step);
            return Self;
        }

        public TChild SetVariable<T>(T variable) where T : class, IEntityVariable
        {
            m_variables.Set<T>(variable);
            return Self;
        }

        protected TChild CreateEmpty()
        {
            m_creationStrategy = new CreateEmptyStrategy();
            return Self;
        }

        protected TChild CreateFromArchetype<T>() where T : IArchetypeDescriptor
        {
            m_creationStrategy = new CreateFromArchetypeStrategy<T>();
            return Self;
        }

        protected TChild CreateFromPrefab(Entity prefabEntity)
        {
            m_creationStrategy = new CreateFromPrefabStrategy(prefabEntity);
            return Self;
        }

        public TChild AddComponentData<T>(T component) where T : struct, IComponentData
        {
            GetOrCreateGenericStep<AddComponentDataStep<T>, T>().SetValue(component);
            return Self;
        }

        public TChild AddComponentObject<T>(T componentObject) where T : Component
        {
            GetOrCreateGenericStep<AddComponentObjectStep<T>, T>().SetValue(componentObject);
            return Self;
        }

        public TChild SetComponentData<T>(T component) where T : struct, IComponentData
        {
            GetOrCreateStep<SetComponentDataStep<T>>().SetValue(component);
            return Self;
        }

        public TChild SetName(string name)
        {
            GetOrCreateStep<SetNameStep>().SetValue(name);
            return Self;
        }

        public TChild SetTranslation(float3 translation)
        {
            GetOrCreateStep<SetTranslationStep>().SetValue(translation);
            return Self;
        }

        public TChild SetRotation(quaternion quaternion)
        {
            GetOrCreateStep<SetRotationStep>().SetValue(quaternion);
            return Self;
        }

        public TChild SetRotation(float3 euler)
        {
            var quaternion = Unity.Mathematics.quaternion.Euler(euler);
            GetOrCreateStep<SetRotationStep>().SetValue(quaternion);
            return Self;
        }

        public TChild SetRotationAngles(float3 eulerAngles)
        {
            var quaternion = Unity.Mathematics.quaternion.Euler(
                math.radians(eulerAngles.x), math.radians(eulerAngles.y), math.radians(eulerAngles.z)
            );
            GetOrCreateStep<SetRotationStep>().SetValue(quaternion);
            return Self;
        }

        public TChild AddSharedComponentData<T>(T component) where T : struct, ISharedComponentData
        {
            GetOrCreateGenericStep<AddSharedComponentDataStep<T>, T>().SetValue(component);
            return Self;
        }

        public T GetOrCreateGenericStep<T, TGenericValue>() where T : IEntityBuilderGenericStep<TGenericValue>
        {
            var singletonStep = m_steps.FirstOrDefault(x => x is IEntityBuilderGenericStep<TGenericValue>);
            if (singletonStep != null)
                return (T) singletonStep;

            var createdSingletonStep = Activator.CreateInstance<T>();
            m_steps.AddLast(createdSingletonStep);
            return createdSingletonStep;
        }

        public T GetOrCreateStep<T>() where T : IEntityBuilderStep
        {
            var singletonStep = m_steps.FirstOrDefault(x => x.GetType() == typeof(T));
            if (singletonStep != null)
                return (T) singletonStep;

            var createdSingletonStep = Activator.CreateInstance<T>();
            m_steps.AddLast(createdSingletonStep);
            return createdSingletonStep;
        }

        public TChild AddBuffer<T>(params T[] elements) where T : struct, IBufferElementData
        {
            var step = GetOrCreateGenericStep<AddBufferStep<T>, T>();
            foreach (var element in elements)
                step.Add(element);
            return Self;
        }

        public TChild AddElementToBuffer<T>(T element) where T : struct, IBufferElementData
        {
            GetOrCreateGenericStep<AddBufferStep<T>, T>().Add(element);
            return Self;
        }

        public TChild AddPostBuildAction(Action<EntityWrapper> postBuildAction)
        {
            m_postBuildActions.AddLast(postBuildAction);
            return Self;
        }

        public TChild SetParent(Entity entity)
        {
            GetOrCreateStep<SetParentStep>().SetValue(entity);
            return Self;
        }

        public TChild SetScale(float scale)
        {
            GetOrCreateStep<SetScaleStep>().SetValue(scale);
            return Self;
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

        public Entity Build(EntityCommandBuffer.Concurrent entityCommandBuffer, int threadId)
        {
            return Build(EntityManagerWrapper.FromJobCommandBuffer(entityCommandBuffer, threadId));
        }

        public virtual Entity Build(EntityManagerWrapper wrapper)
        {
            m_built = true;
            var entity = m_creationStrategy.Create(wrapper, m_variables);

            OnPreBuild(wrapper);

            foreach (var step in m_steps)
                step.Process(wrapper, m_variables, entity);

            OnPostBuild(wrapper, entity);

            if (m_postBuildActions.Count > 0)
            {
                var entityWrapper = EntityWrapper.Wrap(entity);
                foreach (var postBuildAction in m_postBuildActions)
                    postBuildAction.Invoke(entityWrapper);
            }

            return entity;
        }

        protected virtual void OnPreBuild(EntityManagerWrapper wrapper)
        {
        }

        protected virtual void OnPostBuild(EntityManagerWrapper wrapper, Entity entity)
        {
        }

        public EntityWrapper BuildAndWrap(EntityManagerWrapper wrapper)
        {
            return new EntityWrapper(Build(wrapper), wrapper, m_variables);
        }

        public EntityWrapper BuildAndWrap()
        {
            return BuildAndWrap(EntityManagerWrapper.Default);
        }
    }
}