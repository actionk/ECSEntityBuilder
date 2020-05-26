using System;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Plugins.ECSEntityBuilder.Worlds
{
    public class EntityWorldManager
    {
        #region Singleton

        private static EntityWorldManager INSTANCE = new EntityWorldManager();

        static EntityWorldManager()
        {
        }

        private EntityWorldManager()
        {
        }

        public static EntityWorldManager Instance
        {
            get { return INSTANCE; }
        }

#if UNITY_EDITOR
        // for quick play mode entering 
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset()
        {
            INSTANCE = new EntityWorldManager();
        }
#endif

        #endregion

        public World Default { get; private set; }
        public World Client { get; private set; }
        public uint ClientTick => m_clientSimulationSystemGroup.ServerTick;
        public World Server { get; private set; }
        public uint ServerTick => m_serverSimulationSystemGroup.ServerTick;
        public bool HasServerWorld { get; private set; }
        public bool HasClientWorld { get; private set; }

        private ClientSimulationSystemGroup m_clientSimulationSystemGroup;
        private ServerSimulationSystemGroup m_serverSimulationSystemGroup;

        public void Initialize()
        {
            foreach (var world in World.All)
            {
                if (world.GetExistingSystem<ClientSimulationSystemGroup>() != null)
                {
                    m_clientSimulationSystemGroup = world.GetExistingSystem<ClientSimulationSystemGroup>();
                    Client = world;
                    HasClientWorld = true;
                }
                else if (world.GetExistingSystem<ServerSimulationSystemGroup>() != null)
                {
                    m_serverSimulationSystemGroup = world.GetExistingSystem<ServerSimulationSystemGroup>();
                    Server = world;
                    HasServerWorld = true;
                }

                if (world.GetExistingSystem<TransformSystemGroup>() != null)
                    Default = world;
            }
        }

        public World GetWorldByType(WorldType worldType)
        {
            switch (worldType)
            {
                case WorldType.DEFAULT: return Default;
                case WorldType.SERVER: return Server;
                case WorldType.CLIENT: return Client;
            }

            throw new NotImplementedException();
        }
    }
}