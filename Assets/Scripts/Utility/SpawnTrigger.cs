using UnityEngine;
/// <summary>
/// Attach this to an object to allow it to fire trigger events with the player and thereby allow pickups and enemies to spawn, etc.
/// </summary>
public class SpawnTrigger : MonoBehaviour
{
    /**************** VARIABLES *******************/
    [SerializeField] private int roomID;
    /**********************************************/
    
    /***************** METHODS ********************/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EventBroker.SpawnPickupTrigger(roomID);
            EventBroker.SpawnEnemyTrigger(roomID);
            if (roomID == 2)
            {
                GameManager.Instance.PlayMusic(2);
            }
            else if (roomID == 3)
            {
                GameManager.Instance.PlayMusic(3);
                EventBroker.SpawnEnemyTrigger(-1);
            }
            gameObject.SetActive(false);
        }
    }
    /**********************************************/
}
