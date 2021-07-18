using System.Collections;
using UnityEngine;
using Utility;

namespace Gameplay
{
    /// <summary>
    /// When attached to a gameObject, functions as spawn point for pickups.
    /// </summary>
    public class PickupSpawner : MonoBehaviour
    {
        /**************** VARIABLES *******************/
        [SerializeField] private int roomID;
        [SerializeField] private PickupScriptableObject pickupConfig;
        [SerializeField] private RectTransform[] spawnZones;
        /****************** INIT **********************/
        private void Start()
        {

            if (transform.childCount == 0)
            {
                Debug.LogError("Assign a spawnzone for the pickupSpawner. Setting a temporary arbitray spawnpoint.");
                spawnZones = new[] { Instantiate(new GameObject("Spawnzone", typeof(RectTransform)), this.transform).GetComponent<RectTransform>() };
            }
            else
            {
                spawnZones = GetComponentsInChildren<RectTransform>();
                /*
                spawnZones = new RectTransform[transform.childCount];
                
                for (int i = 0; i < spawnZones.Length; i++)
                {
                    spawnZones[i] = transform.GetChild(i).GetComponent<RectTransform>();
                }
                */
            }

            WaveManager.Instance.OnPickupSpawned += SpawnPickup;
        }        
        /******************* LOOP *********************/
    
        /****************** METHODS *******************/
        private void SpawnPickup(int room)
        {
            SpawnPickup(room, pickupConfig.GetRandomPickup());
        }

        private void SpawnPickup(int room, Pickup pickup)
        {
            RectTransform spawnZone = spawnZones[Random.Range(0, spawnZones.Length)];

            Vector3 spawnPos = spawnZone.position + new Vector3(Random.Range(-spawnZone.rect.width / 2, spawnZone.rect.width / 2), Random.Range(-spawnZone.rect.height / 2, spawnZone.rect.height / 2));

            if (room == roomID && Random.Range(0, 2) > 0)
            {
                StartCoroutine(DropHealth());
                Instantiate(pickup, spawnPos, Quaternion.identity);
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
        private void OnDestroy()
        {
            WaveManager.Instance.OnPickupSpawned -= SpawnPickup;
        }
    }
}