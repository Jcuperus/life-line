using UnityEngine;

namespace Utility
{
    /// <summary>
    /// GameObject responsible for triggering new waves
    /// </summary>
    [RequireComponent(typeof(Collider2D), typeof(MusicSource))]
    public class SpawnTrigger : MonoBehaviour
    {
        /**************** VARIABLES *******************/
        [SerializeField] private int roomID;

        private MusicSource musicSource;
        
        public delegate void WaveTriggeredAction(int roomID);
        public static event WaveTriggeredAction OnWaveTriggered;
    
        /**********************************************/
    
        /***************** METHODS ********************/
        private void Awake()
        {
            musicSource = GetComponent<MusicSource>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                OnWaveTriggered?.Invoke(roomID);
                musicSource.Play();
                Destroy(gameObject);
            }
        }
        /**********************************************/
    }
}