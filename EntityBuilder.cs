using System;
using System.Collections.Generic;
using Plugins.ECSEntityBuilder.Archetypes;
using Plugins.ECSEntityBuilder.Steps;
using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;
using Unity.Mathematics;

namespace Plugins.ECSEntityBuilder
{
    public abstract class EntityBuilder<TChild>
    {
        protected EntityBuilderData data = new EntityBuilderData();

        protected abstract TChild Self { get; }

        private bool m_built;
        private readonly EntityVariableMap m_variables = new EntityVariableMap();
        private readonly LinkedList<IEntityBuilderStep> m_steps = new LinkedList<IEntityBuilderStep>();
        private readonly LinkedList<Action<EntityWrapper>> m_postBuildActions = new LinkedList<Action<EntityWrapper>>();

        ~EntityBuilder()
        {
            if (!m_built)
            {
                throw new Exception($"EntityBuilder {Self} was never built");
            }
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
            return AddStep(new CreateEntity());
        }

        protected TChild CreateFromArchetype<T>() where T : IArchetypeDescriptor
        {
            return AddStep(new CreateEntityFromArchetype<T>());
        }

        protected TChild CreateFromPrefab(Entity prefabEntity)
        {
            return AddStep(new Instantiate(prefabEntity));
        }

        public TChild AddComponentData<T>(T component) where T : struct, IComponentData
        {
            return AddStep(new AddComponentData<T>(component));
        }

        public TChild SetComponentData<T>(T component) where T : struct, IComponentData
        {
            return AddStep(new SetComponentData<T>(component));
        }

        public TChild SetName(string name)
        {
            data.name = name;
            return Self;
        }

        public virtual TChild SetTranslation(float3 translation)
        {
            return AddStep(new SetTranslation(translation));
        }

        public TChild SetRotation(quaternion rotation)
        {
            return AddStep(new SetRotation(rotation));
        }

        public TChild SetRotation(float3 euler)
        {
            return AddStep(new SetRotation(quaternion.Euler(euler)));
        }

        public TChild SetRotationAngles(float3 eulerAngles)
        {
            return AddStep(new SetRotation(quaternion.Euler(
                math.radians(eulerAngles.x), math.radians(eulerAngles.y), math.radians(eulerAngles.z)
            )));
        }

        public TChild AddSharedComponentData<T>(T component) where T : struct, ISharedComponentData
        {
            return AddStep(new AddSharedComponentData<T>(component));
        }

        public TChild AddBuffer<T>(params T[] elements) where T : struct, IBufferElementData
        {
            return AddStep(new AddBuffer<T>(elements));
        }

        public TChild AddBuffer<T>(Action<EntityManagerWrapper> callback) where T : struct, IBufferElementData
        {
            return AddStep(new AddBuffer<T>(callback));
        }

        public TChild AddPostBuildAction(Action<EntityWrapper> postBuildAction)
        {
            m_postBuildActions.AddLast(postBuildAction);
            return Self;
        }

        public TChild SetParent(Entity entity)
        {
            AddStep(new SetParent(entity));
            return Self;
        }

        public TChild SetScale(float scale)
        {
            AddStep(new SetScale(scale));
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

            OnPreBuild(wrapper);

            foreach (var step in m_steps)
                step.Process(wrapper, m_variables, ref data);

            OnPostBuild(wrapper, data.entity);

            if (m_postBuildActions.Count > 0)
            {
                var entityWrapper = EntityWrapper.Wrap(data.entity);
                foreach (var postBuildAction in m_postBuildActions)
                    postBuildAction.Invoke(entityWrapper);
            }

            return data.entity;
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