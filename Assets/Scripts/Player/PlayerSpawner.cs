using System;
using UnityEngine;

/// <summary>
/// MonoBehaviour for object which instantiates a player prefab in the scene.
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    /**************** VARIABLES *******************/
    [SerializeField] private PlayerMovement playerPrefab;

    public delegate void PlayerSpawnAction(PlayerMovement playerMovement);
    public event PlayerSpawnAction OnPlayerSpawn;
    /**********************************************/
    
    /***************** METHODS ********************/
    private void Awake()
    {
        SpawnPlayer();
    }
    
    private void SpawnPlayer()
    {
        PlayerMovement player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        OnPlayerSpawn?.Invoke(player);
    }
    /**********************************************/
}
