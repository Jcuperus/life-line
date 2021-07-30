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
        public void SpawnSubWave(SubWave subWave)
        {
            StartCoroutine(SpawnEnemies(subWave));
        }

        protected virtual IEnumerator SpawnEnemies(SubWave subWave)
        {
            for (int i = 0; i < subWave.amount; i++)
            {
                yield return new WaitForSeconds(subWave.delay);
                AbstractEnemy spawnedEnemy = Instantiate(subWave.type);
                spawnedEnemy.transform.position = transform.position + spawnOffset;
            }
        }
        /**********************************************/
    }
}
