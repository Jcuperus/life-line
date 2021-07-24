using System.Collections;
using System.Collections.Generic;
using Animation;
using Gameplay;
using Gameplay.Projectile;
using UnityEngine;
using Utility;

namespace Player
{
    /// <summary>
    /// Behaviour script for player movement.
    /// </summary>
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerController : MonoBehaviour, IProjectileHit
    {
        /**************** VARIABLES *******************/
        [SerializeField] private ActorAnimationController animationController;
        [SerializeField] private HealthBar healthBarPrefab;
        [SerializeField] private int startingHealthAmount = 5;
        [SerializeField] private bool mouseAim = false;
        
        [SerializeField] private float projectileSpawnOffset = 2f;
        
        [SerializeField, Range(0f, 1f)]
        private float healthSegmentWeight = 0.05f;

        [Header("Sound Effects")] 
        [SerializeField] private AudioEvent damageSounds;
        [SerializeField] private AudioEvent shootingSounds;
        [SerializeField] private AudioEvent deathSounds;

        private Rigidbody2D body;
        private AudioSource audioSource;
        private PlayerMovement playerMovement;

        private HealthBar healthBar;
        private LinkedListNode<GameObject> Node { get; set; }
        
        private int healthBarLength;
        private int damageBoost = 0;

        private bool ricochet;
        private bool spreadShot;
        private bool speedShot;
        private bool isAlive = true;
        
        private GameManager.SpeedMultiplierAction speedMultiplierAction;
        private GameManager.RicochetActivatedAction ricochetActivatedAction;
        private GameManager.SpreadShotActivatedAction spreadShotActivatedAction;
        private GameManager.SpeedShotActivatedAction speedShotActivatedAction;
        /**********************************************/

        /******************* INIT *********************/
        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            audioSource = GetComponent<AudioSource>();
            playerMovement = GetComponent<PlayerMovement>();

            Node = new LinkedListNode<GameObject>(gameObject);
            
            speedMultiplierAction =
                (duration, multiplier) => playerMovement.ApplySpeedModifier(multiplier, duration);
            ricochetActivatedAction = duration => StartCoroutine(ApplyRicochet(duration));
            spreadShotActivatedAction = duration => StartCoroutine(ApplySpreadShot(duration));
            speedShotActivatedAction = duration => StartCoroutine(ApplySpeedShot(duration));
            animationController.OnDeathAnimationFinished += () => GameManager.Instance.Death();

            GameManager.OnHealthPickup += SpawnHealthBarSegment;
            GameManager.OnSpeedMultiplierApplied += speedMultiplierAction;
            GameManager.OnRicochetActivated += ricochetActivatedAction;
            GameManager.OnSpreadShotActivated += spreadShotActivatedAction;
            GameManager.OnSpeedShotActivated += speedShotActivatedAction;
        }

        private void Start()
        {
            for (int i = 0; i < startingHealthAmount; i++)
            {
                SpawnHealthBarSegment();
            }
        }

        private void OnDestroy()
        {
            GameManager.OnHealthPickup -= SpawnHealthBarSegment;
            GameManager.OnSpeedMultiplierApplied -= speedMultiplierAction;
            GameManager.OnRicochetActivated -= ricochetActivatedAction;
            GameManager.OnSpreadShotActivated -= spreadShotActivatedAction;
            GameManager.OnSpeedShotActivated -= speedShotActivatedAction;
        }
        /**********************************************/

        /******************* LOOP *********************/
        private void Update()
        {
            if (!isAlive) return;

            playerMovement.weightModifier = Mathf.Max(0.25f, 1f - healthSegmentWeight * healthBarLength);
            
            if (Input.GetButtonDown("Fire1"))
            {
                FireProjectile();
            }

            if (Input.GetButtonDown("Jump"))
            {
                DetachHealthBar();
            }
        }
        /**********************************************/

        /***************** METHODS ********************/
        private void FireProjectile()
        {
            ProjectileFactory.ProjectileTypes projectileType = ricochet
                ? ProjectileFactory.ProjectileTypes.PlayerRicochet
                : ProjectileFactory.ProjectileTypes.Player;

            shootingSounds.Play(audioSource);
            animationController.AttackAnimation.Play();

            int shotAmount = spreadShot ? 3 : 1;

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

                Projectile proj = ProjectileFactory.Instance.Instantiate(projectileType, projectilePosition, shootDirection);

                if (speedShot)
                {
                    proj.velocity *= 2.5f;
                }
                if (damageBoost > 0)
                {
                    proj.damage += damageBoost;
                }
            }
        }

        private IEnumerator ApplyRicochet(float duration)
        {
            ricochet = true;
            yield return new WaitForSeconds(duration);
            ricochet = false;
        }

        private IEnumerator ApplySpreadShot(float duration)
        {
            spreadShot = true;
            yield return new WaitForSeconds(duration);
            spreadShot = false;
        }
        private IEnumerator ApplySpeedShot(float duration)
        {
            speedShot = true;
            yield return new WaitForSeconds(duration);
            speedShot = false;
        }
        
        private void SpawnHealthBarSegment()
        {
            if (healthBar == null)
            {
                healthBar = Instantiate(healthBarPrefab);
                healthBar.AddFirst(Node);
            }
            
            healthBar.SpawnSegment();
            healthBarLength = healthBar.Count;
        }

        private void AttachHealthBar(HealthBar newHealthBar)
        {
            healthBar = newHealthBar;
            healthBar.AddFirst(Node);
            healthBarLength = healthBar.Count;
            damageBoost = 0;
        }

        private void DetachHealthBar()
        {
            if (!HasHealthBar()) return;
            damageBoost = healthBarLength;
            healthBar.RemoveFirst();
            healthBar = null;
            healthBarLength = 0;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Pickup pickup))
            {
                PickupType type = pickup.Type;
                Destroy(pickup.gameObject);
                GameManager.Instance.ResolvePickup(type);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                OnDamage();
                collision.rigidbody.velocity = -collision.rigidbody.velocity;
            }
            else if (!HasHealthBar() && collision.gameObject.TryGetComponent(out HealthBarSegment segment))
            {
                AttachHealthBar(segment.Parent);
            }
        }

        private void OnDamage()
        {
            damageSounds.Play(audioSource);

            if (HasHealthBar() && healthBar.Count > 1)
            {
                healthBar.RemoveLast();
            }
            else
            {
                Death();
            }
        }

        private bool HasHealthBar()
        {
            return healthBar != null && healthBar.IsFirst(Node);
        }

        private void Death()
        {
            if (!isAlive) return;
            
            isAlive = false;
            body.velocity = Vector2.zero;
            animationController.PlayDeathAnimation();
            deathSounds.Play(audioSource);
        }
        
        public void OnProjectileHit(Projectile projectile)
        {
            OnDamage();
        }
        /**********************************************/
    }
}