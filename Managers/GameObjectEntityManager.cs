using System.Collections.Generic;
using UnityEngine;

namespace Plugins.ECSEntityBuilder.Managers
{
    public class GameObjectEntityManager
    {
        public Transform DefaultParentTransform { get; set; }

        private readonly Dictionary<int, GameObject> m_instances = new Dictionary<int, GameObject>();

        public GameObject GetByInstanceID(int instanceId)
        {
            GameObject value;
            return m_instances.TryGetValue(instanceId, out value) ? value : null;
        }

        public GameObject CreateFromPrefab(GameObject prefab, bool enableImmediately = false)
        {
            var gameObject = Object.Instantiate(prefab, DefaultParentTransform);
            if (!enableImmediately)
                gameObject.SetActive(false);

            Add(gameObject);
            return gameObject;
        }

        public void Add(GameObject gameObject)
        {
            m_instances[gameObject.GetInstanceID()] = gameObject;
        }

        public void Destroy(int instanceId)
        {
            var gameObject = m_instances[instanceId];
            m_instances.Remove(instanceId);
            Object.Destroy(gameObject);
        }

#region Singleton

        private static GameObjectEntityManager INSTANCE = new GameObjectEntityManager();

        static GameObjectEntityManager()
        {
        }

        private GameObjectEntityManager()
        {
        }

        public static GameObjectEntityManager Instance
        {
            get { return INSTANCE; }
        }

        public void Remove(GameObject gameObject)
        {
            m_instances.Remove(gameObject.GetInstanceID());
        }

#if UNITY_EDITOR
        // for quick play mode entering 
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset()
        {
            INSTANCE = new GameObjectEntityManager();
        }
#endif

#endregion
    }
}