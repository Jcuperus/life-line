using System;
using UnityEngine;

/// <summary>
/// MonoBehaviour for object which instantiates a player prefab in the scene.
/// </summary>
// Note: it might not be very efficient to use a wholly separate class for this, really all the gameManager would need is the location at which to spawn and the prefab to instantiate, both of which we should be able to give more easily.
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
