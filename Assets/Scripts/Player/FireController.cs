using Gameplay.AttackBehaviour;
using Gameplay.Projectile;
using UnityEngine;
using Utility;
using Utility.Extensions;

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

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            GameManager.OnRicochetActivated += ApplyRicochet;
            GameManager.OnSpreadShotActivated += ApplySpreadShot;
        }

        private void OnDisable()
        {
            GameManager.OnRicochetActivated -= ApplyRicochet;
            GameManager.OnSpreadShotActivated -= ApplySpreadShot;
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

        private void ApplyRicochet(float duration)
        {
            ricochetIsActive = true;
            this.DelayedAction(() => ricochetIsActive = false, duration);
        }

        private void ApplySpreadShot(float duration)
        {
            spreadShotIsActive = true;
            this.DelayedAction(() => spreadShotIsActive = false, duration);
        }
    }
}