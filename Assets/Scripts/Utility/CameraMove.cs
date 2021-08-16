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
        private float currentDuration, currentIntensity;
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

            PlayerController.OnPlayerIsDamaged += ShakeScreen;
        }
        /**********************************************/

        /******************* LOOP *********************/
        private void LateUpdate()
        {
            transform.position = new Vector3(followTransform.position.x, followTransform.position.y, transform.position.z);
            if (currentDuration > 0)
            {
                transform.position += (Vector3)Random.insideUnitCircle * currentIntensity;
            }
        }
        private void Follow(Transform target)
        {
            followTransform = target;
        }
        /**********************************************/

        /***************** METHODS ********************/
        private IEnumerator CameraShake(float duration, float intensity)
        {
            currentDuration+= duration;
            currentIntensity+= intensity;
            while (duration > 0 & Time.timeScale != 0)
            {
                yield return new WaitForEndOfFrame();
                duration -= Time.deltaTime;
            }
            currentDuration -= duration;
            currentIntensity -= intensity;
        }
        public void ShakeScreen(float duration, float intensity)
        {
            StartCoroutine(CameraShake(duration, intensity));
        }
        public void ShakeScreen()
        {
            ShakeScreen(shakeDuration, shakeIntensity); 
        }
        private void OnDestroy()
        {
            PlayerController.OnPlayerIsDamaged -= ShakeScreen;
            StopAllCoroutines();
        }
        /**********************************************/

    }
}