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
    public class PlayerMovement : MonoBehaviour, IProjectileHit
    {
        /**************** VARIABLES *******************/
        [SerializeField] private ActorAnimationController animationController;
        [SerializeField] private HealthBar healthBarPrefab;
        [SerializeField] private int startingHealthAmount = 5;
        [SerializeField] private bool mouseAim = false;

        [Header("Movement Parameters")]
        [SerializeField] private float maxSpeed = 50f;

        [SerializeField] private float maxAcceleration = 75f, maxDeceleration = 75f;
        [SerializeField] private float projectileSpawnOffset = 2f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField, Range(0f, 1f)] private float healthSegmentWeight = 0.05f;

        [Header("Sound Effects")] 
        [SerializeField] private AudioEvent damageSounds;
        [SerializeField] private AudioEvent shootingSounds;
        [SerializeField] private AudioEvent deathSounds;

        private Rigidbody2D body;
        private AudioSource audioSource;

        private HealthBar healthBar;
        private LinkedListNode<GameObject> Node { get; set; }

        private Vector2 inputDirection;
        private Vector2 desiredVelocity;
        private Vector2 velocity;
        private Vector2 lastVelocity;
        private int healthBarLength;
        private float speedMultiplier = 1f;

        private bool ricochet;
        private bool isAlive = true;
        
        private GameManager.SpeedMultiplierAction speedMultiplierAction;
        private GameManager.RicochetActivatedAction ricochetActivatedAction;
        /**********************************************/

        /******************* INIT *********************/
        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            audioSource = GetComponent<AudioSource>();

            Node = new LinkedListNode<GameObject>(gameObject);
            
            speedMultiplierAction =
                (duration, multiplier) => StartCoroutine(ApplySpeedMultiplier(duration, multiplier));
            ricochetActivatedAction = duration => StartCoroutine(ApplyRicochet(duration));
            animationController.OnDeathAnimationFinished += () => GameManager.Instance.Death();

            GameManager.OnHealthPickup += SpawnHealthBarSegment;
            GameManager.OnSpeedMultiplierApplied += speedMultiplierAction;
            GameManager.OnRicochetActivated += ricochetActivatedAction;
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
        }
        /**********************************************/

        /******************* LOOP *********************/
        private void Update()
        {
            if (!isAlive) return;
            
            inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            inputDirection = Vector2.ClampMagnitude(inputDirection, 1f);
            desiredVelocity = inputDirection * maxSpeed;

            if (Input.GetButtonDown("Fire1"))
            {
                FireProjectile();
            }

            if (Input.GetButtonDown("Jump"))
            {
                DetachHealthBar();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                SpawnHealthBarSegment();
            }
        }

        private void FixedUpdate()
        {
            velocity = body.velocity;

            float acceleration = lastVelocity.magnitude < velocity.magnitude ? maxAcceleration : maxDeceleration;
            float weightModifier = Mathf.Max(0.1f, 1f - healthSegmentWeight * healthBarLength);
            float maxSpeedChange = acceleration * speedMultiplier * weightModifier * Time.deltaTime;
            lastVelocity = velocity;
            velocity = Vector2.MoveTowards(velocity, desiredVelocity, maxSpeedChange);

            body.velocity = velocity;

            float inputAngle = VectorHelper.GetAngleFromDirection(inputDirection);
            float smoothRotationAngle = Mathf.LerpAngle(transform.eulerAngles.z, inputAngle,
                Time.deltaTime * rotationSpeed * inputDirection.magnitude);
            transform.rotation = Quaternion.Euler(0f, 0f, smoothRotationAngle);
        }
        /**********************************************/

        /***************** METHODS ********************/
        private void FireProjectile()
        {
            shootingSounds.Play(audioSource);
            animationController.AttackAnimation.Play();

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

            ProjectileFactory.ProjectileTypes projectileType = ricochet
                ? ProjectileFactory.ProjectileTypes.PlayerRicochet
                : ProjectileFactory.ProjectileTypes.Player;
            Vector3 projectilePosition = transform.position + shootDirection * projectileSpawnOffset;
            ProjectileFactory.Instance.Instantiate(projectileType, projectilePosition, shootDirection);
        }

        private IEnumerator ApplySpeedMultiplier(float duration, float multiplier)
        {
            speedMultiplier = multiplier;
            yield return new WaitForSeconds(duration);
            speedMultiplier = 1f;
        }

        private IEnumerator ApplyRicochet(float duration)
        {
            ricochet = true;
            yield return new WaitForSeconds(duration);
            ricochet = false;
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
        }

        private void DetachHealthBar()
        {
            if (!HasHealthBar()) return;
            
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