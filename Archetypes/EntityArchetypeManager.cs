using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Plugins.Framework.Logs;
using Unity.Entities;
using UnityEngine;

namespace Plugins.ECSEntityBuilder.Archetypes
{
    public class EntityArchetypeManager
    {
        private static EntityArchetypeManager INSTANCE = new EntityArchetypeManager();

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

        #if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset()
        {
            INSTANCE = new EntityArchetypeManager();
        }
        #endif

        private Dictionary<Type, EntityArchetype> m_archetypes = new Dictionary<Type, EntityArchetype>();

        public void InitializeArchetypes(Assembly assembly)
        {
            var typesToInitialize = new LinkedList<Type>();
            foreach (Type type in assembly.GetTypes())
            {
                var archetypeAttribute = Attribute.GetCustomAttribute(type, typeof(ArchetypeAttribute));
                if (archetypeAttribute != null) typesToInitialize.AddLast(type);
            }

            InitializeArchetypes(typesToInitialize.ToArray());
        }

        public void InitializeArchetypes(params Type[] types)
        {
            var wrapper = EntityManagerWrapper.Default;
            foreach (var type in types)
            {
                UnityLogger.Info($"Initializing archetype {type}");
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

                try
                {
                    var archetype = wrapper.CreateArchetype(instance.Components);
                    m_archetypes[archetypeType] = archetype;
                    return archetype;
                }
                catch (NotImplementedException)
                {
                    throw new NotImplementedException($"Failed to instantiate archetype from archetype descriptor {archetypeType} because CreateArchetype() method is not supported in this wrapper: {wrapper}");
                }
            }
            catch (NotImplementedException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new NotImplementedException($"Failed to instantiate archetype from archetype descriptor {archetypeType}", e);
            }
        }
    }
}