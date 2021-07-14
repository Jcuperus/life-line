using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Attach this to an object to allow it to fire trigger events with the player and thereby allow pickups and enemies to spawn, etc.
    /// </summary>
    public class SpawnTrigger : MonoBehaviour
    {
        /**************** VARIABLES *******************/
        [SerializeField] private int roomID;
        [SerializeField] private AudioClip roomMusic;
        
        public delegate void WaveTriggeredAction(int roomID);
        public static event WaveTriggeredAction OnWaveTriggered;
    
        /**********************************************/
    
        /***************** METHODS ********************/
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                OnWaveTriggered?.Invoke(roomID);
                GameManager.Instance.PlayMusic(roomMusic);
                Destroy(gameObject);
            }
        }
        /**********************************************/
    }
}