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

        private bool isRicochet, isSpreadShot, isSpeedShot;

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
            GameManager.OnSpeedShotActivated += ApplySpeedShot;
            PlayerController.OnDamageBoostChanged += UpdateDamageBoost;
        }

        private void OnDisable()
        {
            GameManager.OnRicochetActivated -= ricochetActivatedAction;
            GameManager.OnSpreadShotActivated -= spreadShotActivatedAction;
            GameManager.OnSpeedShotActivated -= ApplySpeedShot;
            PlayerController.OnDamageBoostChanged -= UpdateDamageBoost;
        }

        private void UpdateDamageBoost(int amount)
        {
            
        }

        private void Update()
        {
            if (Input.GetButtonDown("Fire1")) FireProjectile();
        }

        private void FireProjectile()
        {
            ProjectileFactory.ProjectileTypes projectileType = isRicochet
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
            
            FireBehaviour fireBehaviour = isSpreadShot ? spreadAttackBehaviour : attackBehaviour;
            fireBehaviour.Execute(projectileType, this, shootDirection);
            
            //TODO: reimplement speedShot, damageBoost
            
            shootingSounds.Play(audioSource);
            playerController.AnimationController.AttackAnimation.Play();
        }

        private IEnumerator ApplyRicochet(float duration)
        {
            isRicochet = true;
            yield return new WaitForSeconds(duration);
            isRicochet = false;
        }

        private IEnumerator ApplySpreadShot(float duration)
        {
            isSpreadShot = true;
            yield return new WaitForSeconds(duration);
            isSpreadShot = false;
        }

        private void ApplySpeedShot(float duration)
        {
            if (isSpeedShot) return;

            StartCoroutine(SpeedShotCoroutine(duration));
        }
        
        private IEnumerator SpeedShotCoroutine(float duration)
        {
            isSpeedShot = true;
            yield return new WaitForSeconds(duration);
            isSpeedShot = false;
        }
    }
}