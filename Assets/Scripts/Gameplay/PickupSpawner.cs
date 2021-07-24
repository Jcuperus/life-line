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
        [SerializeField] private int maxHealthSpawned = 5;
        private Coroutine routine;
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

            WaveManager.Instance.OnPickupWaveTriggered += SpawnPickupWave;
        }        
        /******************* LOOP *********************/
    
        /****************** METHODS *******************/
        private void SpawnPickupWave(int room)
        {
            if (room == roomID)
            {
                if (routine != null)
                {
                    StopCoroutine(routine);
                }
                routine = StartCoroutine(DropHealth());
                SpawnPickup();
            }
            else if(room > roomID)
            {
                StopCoroutine(routine);
                Destroy(gameObject);
            }
        }
        private void SpawnPickup()
        {
            SpawnPickup(pickupConfig.GetRandomPickup());
        }

        private void SpawnPickup(Pickup pickup)
        {
            RectTransform spawnZone = spawnZones[Random.Range(0, spawnZones.Length)];

            Vector3 spawnPos = spawnZone.position + new Vector3(Random.Range(-spawnZone.rect.width / 2, spawnZone.rect.width / 2), Random.Range(-spawnZone.rect.height / 2, spawnZone.rect.height / 2));

            if (pickup != null)
            {
                Instantiate(pickup, spawnPos, Quaternion.identity);
            }
        }
        private IEnumerator DropHealth()
        {
            Pickup healthPickup = pickupConfig.GetPickup(PickupType.HealthUp);
            for (int i = 0; i <= maxHealthSpawned; i++)
            {
                yield return new WaitForSeconds(5);
                SpawnPickup(healthPickup);
            }
        }
        private void OnDestroy()
        {
            WaveManager.Instance.OnPickupWaveTriggered -= SpawnPickupWave;
        }
    }
}