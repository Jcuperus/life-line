using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject[] TEMPWALLS;
    [SerializeField] private PlayerMovement player;
    public PlayerMovement SpawnPlayer()
    {
        GameManager.Instance.wall = TEMPWALLS;
        return Instantiate(player, transform.position, Quaternion.identity);
    }
}
