using System.Collections;
using Gameplay.AttackBehaviour;
using Gameplay.Projectile;
using UnityEngine;
using Utility;

namespace Player
{
    [RequireComponent(typeof(AudioSource))]
    public class FireController : MonoBehaviour
    {
        [SerializeField] private FireBehaviour attackBehaviour, spreadAttackBehaviour;
        [SerializeField] private AudioEvent shootingSounds;
        
        [SerializeField] private bool mouseAim = true;

        private bool ricochetIsActive, spreadShotIsActive;

        private PlayerController playerController;
        private AudioSource audioSource;
        
        private GameManager.RicochetActivatedAction ricochetActivatedAction;
        private GameManager.SpreadShotActivatedAction spreadShotActivatedAction;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            audioSource = GetComponent<AudioSource>();
            
            ricochetActivatedAction = duration => StartCoroutine(ApplyRicochet(duration));
            spreadShotActivatedAction = duration => StartCoroutine(ApplySpreadShot(duration));
        }

        private void OnEnable()
        {
            GameManager.OnRicochetActivated += ricochetActivatedAction;
            GameManager.OnSpreadShotActivated += spreadShotActivatedAction;
        }

        private void OnDisable()
        {
            GameManager.OnRicochetActivated -= ricochetActivatedAction;
            GameManager.OnSpreadShotActivated -= spreadShotActivatedAction;
        }

        private void Update()
        {
            if (Input.GetButtonDown("Fire1")) FireProjectile();
        }

        private void FireProjectile()
        {
            ProjectileFactory.ProjectileTypes projectileType = ricochetIsActive
                ? ProjectileFactory.ProjectileTypes.PlayerRicochet
                : ProjectileFactory.ProjectileTypes.Player;
            
            Vector3 shootDirection;
            
            if (mouseAim)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f;
                shootDirection = (mousePosition - transform.position).normalized;
            }
            else
            {
                shootDirection = VectorHelper.GetDirectionFromAngle(transform.eulerAngles.z);
            }
            
            FireBehaviour fireBehaviour = spreadShotIsActive ? spreadAttackBehaviour : attackBehaviour;
            fireBehaviour.Execute(projectileType, this, shootDirection);
            
            shootingSounds.Play(audioSource);
            playerController.AnimationController.AttackAnimation.Play();
        }

        private IEnumerator ApplyRicochet(float duration)
        {
            ricochetIsActive = true;
            yield return new WaitForSeconds(duration);
            ricochetIsActive = false;
        }

        private IEnumerator ApplySpreadShot(float duration)
        {
            spreadShotIsActive = true;
            yield return new WaitForSeconds(duration);
            spreadShotIsActive = false;
        }
    }
}