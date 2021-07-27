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
        [SerializeField] private float shakeDuration = 0.1f, shakeIntensity = 0.5f;

        private float currentShakeDuration;
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

            PlayerController.OnPlayerIsDamaged += Shake;
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
                
                if (currentShakeDuration > 0)
                {
                    transform.position += (Vector3) Random.insideUnitCircle * shakeIntensity;
                    currentShakeDuration -= Time.deltaTime;
                }
                
                yield return null;
            }
        }
        /**********************************************/
        
        /***************** METHODS ********************/
        private void Shake()
        {
            currentShakeDuration = shakeDuration;
        }
        /**********************************************/

    }
}