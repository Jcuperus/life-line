using System.Collections;
using UnityEngine;
/// <summary>
/// Behaviour class for moving the camera along with a target
/// </summary>
public class CameraMove : MonoBehaviour
{
    /**************** VARIABLES *******************/
    [SerializeField] private Transform followTransform;
    /**********************************************/
    
    /******************* INIT *********************/
    private void Awake()
    {
        //TODO: maybe make this more generic somehow
        PlayerSpawner.OnPlayerSpawn += player =>
        {
            followTransform = player.transform;
            StartCoroutine(FollowTarget());
        };
    }
    /**********************************************/
    
    /******************* LOOP *********************/
    private IEnumerator FollowTarget()
    {
        while (isActiveAndEnabled)
        {
            transform.position = new Vector3(followTransform.position.x, followTransform.position.y, transform.position.z);
            yield return null;
        }
    }
    /**********************************************/
}