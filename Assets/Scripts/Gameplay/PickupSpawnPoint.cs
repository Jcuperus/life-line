using System.Collections;
using UnityEngine;
using Utility;

namespace Gameplay
{
    /// <summary>
    /// When attached to a gameObject, functions as spawn point for pickups.
    /// </summary>
    public class PickupSpawnPoint : MonoBehaviour
    {
        /**************** VARIABLES *******************/
        [SerializeField] private int roomID;
        [SerializeField] private PickupScriptableObject pickupConfig;
        
        private Vector3[] spawnPoints;
        /**********************************************/
    
        /******************* LOOP *********************/
        private void Start()
        {

            if (transform.childCount == 0)
            {
                spawnPoints = new[] {transform.position};
            }
            else
            {
                spawnPoints = new Vector3[transform.childCount];
                
                for (int i = 0; i < spawnPoints.Length; i++)
                {
                    spawnPoints[i] = transform.GetChild(i).position;
                }
            }

            WaveManager.Instance.OnPickupSpawned += SpawnPickup;
        }

        private void OnDestroy()
        {
            WaveManager.Instance.OnPickupSpawned -= SpawnPickup;
        }
        /**********************************************/
    
        /****************** METHODS *******************/
        private void SpawnPickup(int room)
        {
            SpawnPickup(room, pickupConfig.GetRandomPickup());
            StartCoroutine(DropHealth());
        }

        private void SpawnPickup(int room, Pickup pickup)
        {
            Vector3 spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
            if (room == roomID && Random.Range(0, 2) > 0)
            {
                Instantiate(pickup, spawnPosition, Quaternion.identity);
            }
        }
    
        private IEnumerator DropHealth()
        {
            Pickup healthPickup = pickupConfig.GetPickup(PickupType.HealthUp);
            
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(10);
                SpawnPickup(roomID, healthPickup);
            }
        }
        /**********************************************/
    }
}