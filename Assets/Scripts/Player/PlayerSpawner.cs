using UnityEngine;

/// <summary>
/// MonoBehaviour for object which instantiates a player prefab in the scene.
/// </summary>
// Note: it might not be very efficient to use a wholly separate class for this, really all the gameManager would need is the location at which to spawn and the prefab to instantiate, both of which we should be able to give more easily.
public class PlayerSpawner : MonoBehaviour
{
    /**************** VARIABLES *******************/
    [SerializeField] private GameObject[] barriers; //temporary; possibly change to different system, probably at least move to game manager
    [SerializeField] private PlayerMovement playerPrefab;
    /**********************************************/
    
    /***************** METHODS ********************/
    public PlayerMovement SpawnPlayer()
    {
        //TODO: might be neater to do this via the event broker than with a public method, as this could easily go wrong ,spawning multiple players
        GameManager.Instance.barriers = barriers;
        return Instantiate(playerPrefab, transform.position, Quaternion.identity);
    }
    /**********************************************/
}
