using System.Collections;
using Gameplay.Projectile;
using UnityEngine;
using Utility;

namespace Player
{
    [RequireComponent(typeof(AudioSource))]
    public class FireController : MonoBehaviour
    {
        [SerializeField] private AudioEvent shootingSounds;

        [SerializeField] private float projectileSpawnOffset = 2f;
        [SerializeField] private bool mouseAim = true;

        private bool isRicochet, isSpreadShot, isSpeedShot;
        private int damageBoost;

        private PlayerController playerController;
        private AudioSource audioSource;
        
        private GameManager.RicochetActivatedAction ricochetActivatedAction;
        private GameManager.SpreadShotActivatedAction spreadShotActivatedAction;
        private GameManager.SpeedShotActivatedAction speedShotActivatedAction;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            audioSource = GetComponent<AudioSource>();
            
            ricochetActivatedAction = duration => StartCoroutine(ApplyRicochet(duration));
            spreadShotActivatedAction = duration => StartCoroutine(ApplySpreadShot(duration));
            speedShotActivatedAction = duration => StartCoroutine(ApplySpeedShot(duration));
        }

        private void OnEnable()
        {
            GameManager.OnRicochetActivated += ricochetActivatedAction;
            GameManager.OnSpreadShotActivated += spreadShotActivatedAction;
            GameManager.OnSpeedShotActivated += speedShotActivatedAction;
            PlayerController.OnDamageBoostChanged += UpdateDamageBoost;
        }

        private void OnDisable()
        {
            GameManager.OnRicochetActivated -= ricochetActivatedAction;
            GameManager.OnSpreadShotActivated -= spreadShotActivatedAction;
            GameManager.OnSpeedShotActivated -= speedShotActivatedAction;
            PlayerController.OnDamageBoostChanged -= UpdateDamageBoost;
        }

        private void UpdateDamageBoost(int amount)
        {
            damageBoost = amount;
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

            shootingSounds.Play(audioSource);
            playerController.AnimationController.AttackAnimation.Play();

            int shotAmount = isSpreadShot ? 3 : 1;

            for (int i = 0; i < shotAmount; i++)
            {
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

                if (i == 1)
                {
                    shootDirection += VectorHelper.GetDirectionFromAngle(30);
                }
                else if (i == 2)
                {
                    shootDirection -= VectorHelper.GetDirectionFromAngle(-30);
                }

                Vector3 projectilePosition = transform.position + shootDirection * projectileSpawnOffset;
                Projectile projectile = ProjectileFactory.Instance.Instantiate(projectileType, projectilePosition, shootDirection);

                if (isSpeedShot)
                {
                    projectile.velocity *= 2.5f;
                }
                
                projectile.damage += damageBoost;
            }
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
        private IEnumerator ApplySpeedShot(float duration)
        {
            isSpeedShot = true;
            yield return new WaitForSeconds(duration);
            isSpeedShot = false;
        }
    }
}