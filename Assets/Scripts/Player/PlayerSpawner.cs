using UnityEngine;

namespace Player
{
    /// <summary>
    /// MonoBehaviour for object which instantiates a player prefab in the scene.
    /// </summary>
    public class PlayerSpawner : MonoBehaviour
    {
        /**************** VARIABLES *******************/
        [SerializeField] private PlayerController playerPrefab;

        public delegate void PlayerSpawnAction(PlayerController playerController);
        public event PlayerSpawnAction OnPlayerSpawn;
        /**********************************************/
    
        /***************** METHODS ********************/
        private void Awake()
        {
            SpawnPlayer();
        }
    
        private void SpawnPlayer()
        {
            PlayerController player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
            OnPlayerSpawn?.Invoke(player);
        }
        /**********************************************/
    }
}