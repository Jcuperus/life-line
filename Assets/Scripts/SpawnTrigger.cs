using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    [SerializeField] private int roomID;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EventBroker.SpawnPickupTrigger(roomID);
            EventBroker.SpawnEnemyTrigger(roomID);
            if (roomID == 2)
            {
                GameManager.Instance.SetMusic(2);
            }
            else if (roomID == 3)
            {
                GameManager.Instance.SetMusic(3);
                EventBroker.SpawnEnemyTrigger(-1);
            }
            gameObject.SetActive(false);
        }
    }
    

}
