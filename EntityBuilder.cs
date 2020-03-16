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

        public TChild AddComponentData<T>(T component) where T : struct, IComponentData
        {
            return AddStep(new AddComponentData<T>(component));
        }

        public TChild SetComponentData<T>(T component) where T : struct, IComponentData
        {
            return AddStep(new SetComponentData<T>(component));
        }

        public TChild SetPosition(float3 position)
        {
            return AddStep(new SetPosition(position));
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

        public Entity Build()
        {
            return Build(EntityManagerWrapper.Default);
        }

        public Entity Build(EntityCommandBuffer entityCommandBuffer)
        {
            return Build(EntityManagerWrapper.FromCommandBuffer(entityCommandBuffer));
        }

        public Entity Build(EntityCommandBuffer.Concurrent entityCommandBuffer, int threadId)
        {
            return Build(EntityManagerWrapper.FromJobCommandBuffer(entityCommandBuffer, threadId));
        }

        public Entity Build(EntityManagerWrapper wrapper)
        {
            m_built = true;

            foreach (var step in m_steps)
                step.Process(wrapper, m_variables, ref data);

            return data.entity;
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