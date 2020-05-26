using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.ECSEntityBuilder.Archetypes;
using Plugins.ECSEntityBuilder.InstantiationStrategy;
using Plugins.ECSEntityBuilder.Steps;
using Plugins.ECSEntityBuilder.Variables;
using Plugins.ECSEntityBuilder.Worlds;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Plugins.ECSEntityBuilder
{
    public abstract class EntityBuilder<TChild> where TChild : EntityBuilder<TChild>
    {
        public delegate void OnPreBuildHandler();

        public delegate void OnPostBuildHandler(EntityWrapper entityWrapper);

        protected bool m_built;
        protected IEntityCreationStrategy m_creationStrategy = CreateEmptyStrategy.DEFAULT;
        protected EntityVariableMap m_variables;
        protected readonly LinkedList<IEntityBuilderStep> m_steps = new LinkedList<IEntityBuilderStep>();

        protected event OnPreBuildHandler preBuild;
        protected event OnPostBuildHandler postBuild;

        ~EntityBuilder()
        {
            if (!m_built)
            {
                throw new Exception($"EntityBuilder {typeof(TChild)} was never built");
            }
        }

        public bool HasStep<T, TGeneric>() where T : IEntityBuilderGenericStep<TGeneric>
        {
            return m_steps.FirstOrDefault(x => x is IEntityBuilderGenericStep<TGeneric>) != null;
        }

        public TChild AddStep(IEntityBuilderStep step)
        {
            m_steps.AddLast(step);
            return (TChild) this;
        }

        public TChild SetVariable<T>(T variable) where T : class, IEntityVariable
        {
            if (m_variables == null)
                m_variables = new EntityVariableMap();

            m_variables.Set(variable);
            return (TChild) this;
        }

        protected TChild CreateEmpty()
        {
            m_creationStrategy = new CreateEmptyStrategy();
            return (TChild) this;
        }

        protected TChild CreateFromArchetype<T>() where T : IArchetypeDescriptor
        {
            m_creationStrategy = new CreateFromArchetypeStrategy<T>(WorldType.DEFAULT);
            return (TChild) this;
        }

        protected TChild CreateFromArchetype<T>(WorldType worldType) where T : IClientServerArchetypeDescriptor
        {
            m_creationStrategy = new CreateFromArchetypeStrategy<T>(worldType);
            return (TChild) this;
        }

        protected TChild CreateFromPrefab(Entity prefabEntity)
        {
            m_creationStrategy = new CreateFromPrefabStrategy(prefabEntity);
            return (TChild) this;
        }

        public TChild AddComponent<T>() where T : struct, IComponentData
        {
            GetOrCreateGenericStep<AddComponentStep<T>, T>();
            return (TChild) this;
        }

        public TChild AddComponentData<T>(T component) where T : struct, IComponentData
        {
            GetOrCreateGenericStep<AddComponentDataStep<T>, T>().SetValue(component);
            return (TChild) this;
        }

        public TChild AddComponentObject<T>(T componentObject) where T : Component
        {
            GetOrCreateGenericStep<AddComponentObjectStep<T>, T>().SetValue(componentObject);
            return (TChild) this;
        }

        public TChild SetComponentData<T>(T component) where T : struct, IComponentData
        {
            GetOrCreateStep<SetComponentDataStep<T>>().SetValue(component);
            return (TChild) this;
        }

        public TChild SetName(string name)
        {
            GetOrCreateStep<SetNameStep>().SetValue(name);
            return (TChild) this;
        }

        public TChild SetTranslation(float3 translation)
        {
            GetOrCreateStep<SetTranslationStep>().SetValue(translation);
            return (TChild) this;
        }

        public TChild SetRotation(quaternion quaternion)
        {
            GetOrCreateStep<SetRotationStep>().SetValue(quaternion);
            return (TChild) this;
        }

        public TChild SetRotation(float3 euler)
        {
            var quaternion = Unity.Mathematics.quaternion.Euler(euler);
            GetOrCreateStep<SetRotationStep>().SetValue(quaternion);
            return (TChild) this;
        }

        public TChild SetRotationAngles(float3 eulerAngles)
        {
            var quaternion = Unity.Mathematics.quaternion.Euler(
                math.radians(eulerAngles.x), math.radians(eulerAngles.y), math.radians(eulerAngles.z)
            );
            GetOrCreateStep<SetRotationStep>().SetValue(quaternion);
            return (TChild) this;
        }

        public TChild AddSharedComponentData<T>(T component) where T : struct, ISharedComponentData
        {
            GetOrCreateGenericStep<AddSharedComponentDataStep<T>, T>().SetValue(component);
            return (TChild) this;
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
            return (TChild) this;
        }

        public TChild AddElementToBuffer<T>(T element) where T : struct, IBufferElementData
        {
            GetOrCreateGenericStep<AddBufferStep<T>, T>().Add(element);
            return (TChild) this;
        }

        public TChild SetParent(Entity entity)
        {
            GetOrCreateStep<SetParentStep>().SetValue(entity);
            return (TChild) this;
        }

        public TChild SetScale(float scale)
        {
            GetOrCreateStep<SetScaleStep>().SetValue(scale);
            return (TChild) this;
        }

        public Entity Build()
        {
            return Build(EntityManagerWrapper.Default);
        }

        public Entity Build(EntityManager entityManager)
        {
            return Build(new EntityManagerWrapper(entityManager));
        }

        public Entity Build(WorldType worldType)
        {
            return Build(new EntityManagerWrapper(EntityWorldManager.Instance.GetWorldByType(worldType).EntityManager));
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

            preBuild?.Invoke();

            OnPreBuild(wrapper);

            foreach (var step in m_steps)
                step.Process(wrapper, m_variables, entity);

            OnPostBuild(wrapper, entity);

            postBuild?.Invoke(EntityWrapper.Wrap(entity, wrapper));

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