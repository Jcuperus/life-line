using System;
using System.Collections;
using Spine.Unity;
using UnityEngine;

/// <summary>
/// Behaviour class for moving the camera along with a target
/// </summary>
public class CameraMove : MonoBehaviour
{
    /**************** VARIABLES *******************/
    [SerializeField] private Transform followTransform;

    private PlayerSpawner.PlayerSpawnAction playerSpawnAction;
    /**********************************************/
    
    /******************* INIT *********************/
    private void Awake()
    {
        playerSpawnAction = player => Follow(player.transform);
        FindObjectOfType<PlayerSpawner>().OnPlayerSpawn += playerSpawnAction;
    }
    /**********************************************/
    
    /******************* LOOP *********************/
    private void Follow(Transform target)
    {
        followTransform = target;
        StartCoroutine(FollowTargetCoroutine());
    }
    
    private IEnumerator FollowTargetCoroutine()
    {
        while (isActiveAndEnabled)
        {
            transform.position = new Vector3(followTransform.position.x, followTransform.position.y, transform.position.z);
            yield return null;
        }
    }
    /**********************************************/
}