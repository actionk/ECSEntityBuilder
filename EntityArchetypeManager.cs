using System;
using System.Collections.Generic;
using Plugins.ECSEntityBuilder.Archetypes;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder
{
    public class EntityArchetypeManager
    {
        private static readonly EntityArchetypeManager INSTANCE = new EntityArchetypeManager();

        static EntityArchetypeManager()
        {
        }

        private EntityArchetypeManager()
        {
        }

        public static EntityArchetypeManager Instance
        {
            get { return INSTANCE; }
        }

        private Dictionary<Type, EntityArchetype> m_archetypes = new Dictionary<Type, EntityArchetype>();

        public void InitializeArchetypes(params Type[] types)
        {
            var wrapper = EntityManagerWrapper.Default;
            foreach (var type in types)
            {
                GetOrCreateArchetype(wrapper, type);
            }
        }

        public EntityArchetype GetOrCreateArchetype<T>(EntityManagerWrapper wrapper) where T : IArchetypeDescriptor
        {
            var archetypeType = typeof(T);
            return GetOrCreateArchetype(wrapper, archetypeType);
        }

        private EntityArchetype GetOrCreateArchetype(EntityManagerWrapper wrapper, Type archetypeType)
        {
            if (m_archetypes.ContainsKey(archetypeType))
                return m_archetypes[archetypeType];

            try
            {
                var instance = Activator.CreateInstance(archetypeType) as IArchetypeDescriptor;
                if (instance == null)
                {
                    throw new NotImplementedException($"Archetype descriptor {archetypeType} should implement {typeof(IArchetypeDescriptor)} interface and have an empty constructor");
                }

                var archetype = wrapper.CreateArchetype(instance.Components);
                m_archetypes[archetypeType] = archetype;
                return archetype;
            }
            catch (Exception e)
            {
                throw new NotImplementedException($"Failed to instantiate archetype from archetype descriptor {archetypeType}", e);
            }
        }
    }
}