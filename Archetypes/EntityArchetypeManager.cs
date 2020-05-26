using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Plugins.ECSEntityBuilder.Worlds;
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

        private struct EntityArchetypeHolder
        {
            public EntityArchetype defaultArchetype;
            public EntityArchetype clientArchetype;
            public EntityArchetype serverArchetype;
        }

        private readonly Dictionary<Type, EntityArchetypeHolder> m_archetypes = new Dictionary<Type, EntityArchetypeHolder>();


        public void InitializeArchetypes(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var archetypeAttribute = Attribute.GetCustomAttribute(type, typeof(ArchetypeAttribute));
                if (archetypeAttribute != null) GetOrCreateArchetype(type);
            }
        }

        public void InitializeArchetypes(params Type[] types)
        {
            foreach (var type in types)
                GetOrCreateArchetype(type);
        }

        public EntityArchetype GetOrCreateArchetype<T>() where T : IArchetypeDescriptor
        {
            return GetOrCreateArchetype<T>(WorldType.DEFAULT);
        }

        public EntityArchetype GetOrCreateArchetype<T>(WorldType worldType) where T : IArchetypeDescriptor
        {
            var archetypeType = typeof(T);
            var entityArchetypeHolder = GetOrCreateArchetype(archetypeType);
            switch (worldType)
            {
                case WorldType.DEFAULT:
                    return entityArchetypeHolder.defaultArchetype;
                case WorldType.CLIENT:
                    return entityArchetypeHolder.clientArchetype;
                case WorldType.SERVER:
                    return entityArchetypeHolder.serverArchetype;
            }

            throw new NotImplementedException();
        }

        private EntityArchetypeHolder GetOrCreateArchetype(Type archetypeType)
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

                if (instance is IClientServerArchetypeDescriptor clientServerInstance)
                {
                    var clientComponents = clientServerInstance.Components.Concat(clientServerInstance.ClientOnlyComponents).ToArray();
                    var clientArchetype = EntityWorldManager.Instance.Client.EntityManager.CreateArchetype(clientComponents);

                    var serverComponents = clientServerInstance.Components.Concat(clientServerInstance.ServerOnlyComponents).ToArray();
                    var serverArchetype = EntityWorldManager.Instance.Server.EntityManager.CreateArchetype(serverComponents);

                    var archetypeHolder = new EntityArchetypeHolder
                    {
                        clientArchetype = clientArchetype,
                        serverArchetype = serverArchetype
                    };
                    m_archetypes[archetypeType] = archetypeHolder;
                    return archetypeHolder;
                }
                else
                {
                    var archetype = EntityWorldManager.Instance.Default.EntityManager.CreateArchetype(instance.Components);

                    var archetypeHolder = new EntityArchetypeHolder
                    {
                        defaultArchetype = archetype
                    };
                    m_archetypes[archetypeType] = archetypeHolder;
                    return archetypeHolder;
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