using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly List<T> pool = new List<T>();
        private readonly T objectInstance;

        public ObjectPool(T objectInstance, int initialPoolSize = 10)
        {
            this.objectInstance = objectInstance;
            
            for (int i = 0; i < initialPoolSize; i++)
            {
                AddObjectToPool();
            }
        }

        public T GetObject()
        {
            foreach (T pooledObject in pool)
            {
                if (!pooledObject.gameObject.activeInHierarchy)
                {
                    return pooledObject;
                }
            }

            return AddObjectToPool();
        }

        private T AddObjectToPool()
        {
            T pooledObject = Object.Instantiate(objectInstance);
            pooledObject.gameObject.SetActive(false);
            pool.Add(pooledObject);
            return pooledObject;
        }
    }
}