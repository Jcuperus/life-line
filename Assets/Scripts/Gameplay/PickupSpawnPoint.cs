using System.Collections;
using UnityEngine;
/// <summary>
/// When attached to a gameObject, functions as spawn point for pickups.
/// </summary>
// Note: might do this differently, such as with a plane, which indicates an area in which pickups may be spawned at random, instead of in the same location each time
public class PickupSpawnPoint : MonoBehaviour
{
    /**************** VARIABLES *******************/
    [SerializeField] private int roomID;
    /**********************************************/
    
    /******************* LOOP *********************/
    private void Start()
    {
        EventBroker.SpawnPickupEvent += SpawnPickup;
    }
    /**********************************************/
    
    /****************** METHODS *******************/
    private void SpawnPickup(int room, int index = -1)
    {
        
        if (room == roomID & Random.Range(0, 2) > 0)
        {
            if (index == -1) { index = Random.Range(0, GameManager.Instance.GetPickups.Length); }
            //TODO: something is causing a nullReference here
            Instantiate(GameManager.Instance.GetPickups[index], transform.position, Quaternion.identity);
            StartCoroutine(DropHealth());
        }
    }
    
    private IEnumerator DropHealth()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(10);
            SpawnPickup(roomID, 0);
        }
    }
    /**********************************************/
}