using System.Collections;
using UnityEngine;
using Utility;

namespace Enemies
{
    /// <summary>
    /// Behaviour for glitched walls, functioning as enemy spawn points. 
    /// </summary>
    public class EnemySpawnPoint : MonoBehaviour
    {
        /**************** VARIABLES *******************/
        [SerializeField] private Vector3 spawnOffset = Vector3.zero;

        public int roomID;
        /**********************************************/
    
        /******************* INIT *********************/
        private void Start()
        {
            WaveManager.Instance.RegisterSpawnPoint(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + spawnOffset, 0.5f);
        }
        /**********************************************/
    
        /***************** METHODS ********************/
        public virtual IEnumerator SpawnEnemy(AbstractEnemy prefab)
        {
                yield return new WaitForSeconds(0);
                AbstractEnemy spawnedEnemy = Instantiate(prefab);
                spawnedEnemy.transform.position = transform.position + spawnOffset;
        }
        /**********************************************/
    }
}
