using System;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Plugins.ECSEntityBuilder.Worlds
{
    public class WorldManager
    {
        #region Singleton

        private static WorldManager INSTANCE = new WorldManager();

        static WorldManager()
        {
        }

        private WorldManager()
        {
        }

        public static WorldManager Instance
        {
            get { return INSTANCE; }
        }

#if UNITY_EDITOR
        // for quick play mode entering 
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset()
        {
            INSTANCE = new WorldManager();
        }
#endif

        #endregion

        public World Default { get; private set; }
        public World Client { get; private set; }
        public World Server { get; private set; }

        public void Initialize()
        {
            foreach (var world in World.All)
            {
                if (world.GetExistingSystem<ClientSimulationSystemGroup>() != null)
                    Client = world;
                else if (world.GetExistingSystem<ServerSimulationSystemGroup>() != null)
                    Server = world;

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