using System.Collections;
using Player;
using UnityEngine;

namespace Utility
{
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
            var spawner = FindObjectOfType<PlayerSpawner>();

            if (spawner != null)
            {
                spawner.OnPlayerSpawn += player => Follow(player.transform);
            }
            else
            {
                Follow(followTransform);
            }
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
}