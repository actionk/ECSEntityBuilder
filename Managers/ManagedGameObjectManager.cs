using System.Collections.Generic;
using UnityEngine;

namespace Plugins.ECSEntityBuilder.Managers
{
    public class ManagedGameObjectManager
    {
        private readonly Dictionary<int, GameObject> m_instances = new Dictionary<int, GameObject>();
        private Transform m_defaultParentTransform;

        public void Initialize()
        {
            m_defaultParentTransform = Object.FindObjectOfType<ManagedGameObjectContainer>().transform;
        }

        public GameObject GetByInstanceID(int instanceId)
        {
            GameObject value;
            return m_instances.TryGetValue(instanceId, out value) ? value : null;
        }

        public GameObject CreateFromPrefab(GameObject prefab, bool enableImmediately = false)
        {
            var gameObject = Object.Instantiate(prefab, m_defaultParentTransform);
            if (!enableImmediately)
                gameObject.SetActive(false);

            Add(gameObject);
            return gameObject;
        }

        public GameObject CreateFromPrefab(GameObject prefab, Vector3 position, Quaternion rotation, bool enableImmediately = false)
        {
            var gameObject = Object.Instantiate(prefab, position, rotation, m_defaultParentTransform);
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

        private static ManagedGameObjectManager INSTANCE = new ManagedGameObjectManager();

        static ManagedGameObjectManager()
        {
        }

        private ManagedGameObjectManager()
        {
        }

        public static ManagedGameObjectManager Instance
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
            INSTANCE = new ManagedGameObjectManager();
        }
#endif

#endregion
    }
}