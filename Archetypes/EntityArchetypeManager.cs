using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Entities;
using UnityEngine;

namespace Plugins.ECSEntityBuilder.Archetypes
{
    public class EntityArchetypeManager
    {
#region Singleton

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
        // for quick play mode entering 
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset()
        {
            INSTANCE = new EntityArchetypeManager();
        }
#endif

#endregion

        private readonly Dictionary<Type, EntityArchetype> m_archetypes = new Dictionary<Type, EntityArchetype>();

        public void InitializeArchetypes(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (Attribute.GetCustomAttribute(type, typeof(ArchetypeAttribute)) is ArchetypeAttribute archetypeAttribute)
                    GetOrCreateArchetype(type, archetypeAttribute);
            }
        }

        public void InitializeArchetypes(params Type[] types)
        {
            foreach (var type in types)
                GetOrCreateArchetype(type);
        }

        public EntityArchetype GetOrCreateArchetype<T>() where T : IArchetypeDescriptor
        {
            var archetypeType = typeof(T);
            if (archetypeType.IsAbstract)
            {
                throw new NotImplementedException($"It's impossible to create an archetype from an abstract class: {archetypeType}");
            }

            return GetOrCreateArchetype(archetypeType);
        }

        private EntityArchetype GetOrCreateArchetype(Type archetypeType, ArchetypeAttribute attribute = null)
        {
            if (m_archetypes.ContainsKey(archetypeType))
                return m_archetypes[archetypeType];

            if (attribute == null)
                attribute = Attribute.GetCustomAttribute(archetypeType, typeof(ArchetypeAttribute)) as ArchetypeAttribute;

            try
            {
                var instance = Activator.CreateInstance(archetypeType) as IArchetypeDescriptor;
                if (instance == null)
                {
                    throw new NotImplementedException(
                        $"Archetype descriptor {archetypeType} should implement {typeof(IArchetypeDescriptor)} interface and have an empty constructor");
                }

                if (instance is ICustomArchetypeDescriptor customArchetypeDescriptor)
                {
                    var components = customArchetypeDescriptor.Components.Concat(customArchetypeDescriptor.CustomComponents).ToArray();

                    var archetype = World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(components);
                    m_archetypes[archetypeType] = archetype;
                    return archetype;
                }
                else
                {
                    var archetype = World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(instance.Components);
                    m_archetypes[archetypeType] = archetype;
                    return archetype;
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