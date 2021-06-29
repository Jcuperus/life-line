using System.Collections;
using UnityEngine;

public class PickupSpawnPoint : MonoBehaviour
{
    [SerializeField] private int roomID;
    private void Start()
    {
        EventBroker.SpawnPickupEvent += SpawnPickup;
    }
    private void SpawnPickup(int room, int index = -1)
    {
        if (room == roomID & Random.Range(0, 2)>0)
        {
            if (index == -1) { index = Random.Range(0, GameManager.Instance.GetPickups.Length); }
            Instantiate<Pickup>(GameManager.Instance.GetPickups[index], transform.position, Quaternion.identity);
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
}