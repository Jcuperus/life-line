using UnityEngine;

namespace Rooms
{
    public class EnemySpawner : MonoBehaviour
    {
        public GameObject enemyPrefab;
        
        public void OnAnimationFinished()
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.Euler(Vector3.zero));
            Destroy(gameObject);
        }
    }
}

