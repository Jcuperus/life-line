using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Gameplay.Projectile;
using UnityEngine;
using Utility;

namespace Player
{
    /// <summary>
    /// Behaviour script for player movement.
    /// </summary>
    [RequireComponent(typeof(HealthBar))]
    public class PlayerMovement : MonoBehaviour, IProjectileHit
    {
        /**************** VARIABLES *******************/
        [SerializeField] private int startingHealthAmount = 5;
        [SerializeField] private bool mouseAim = false;
        
        [Header("Movement Parameters")]
        [SerializeField] private float maxSpeed = 50f;

        [SerializeField] private float maxAcceleration = 75f, maxDeceleration = 75f;
        [SerializeField] private float projectileSpawnOffset = 2f;
        [SerializeField] private float rotationSpeed = 15f;

        [Header("Prefabs & Assets")] 
        [SerializeField] private AudioEvent damageSounds;
        [SerializeField] private AudioEvent shootingSounds;
        [SerializeField] private AudioEvent deathSounds;

        private ProjectileFactory projectileFactory;
        private Rigidbody2D body;
        private AudioSource audioSource;

        private HealthBar healthBar;
        private LinkedListNode<GameObject> Node { get; set; }

        private Vector2 inputDirection;
        private Vector2 desiredVelocity;
        private Vector2 velocity;
        private Vector2 lastVelocity;
        private float speedMultiplier = 1f;

        private bool ricochet;

        private Action healthPickupAction;
        private GameManager.SpeedMultiplierAction speedMultiplierAction;
        private GameManager.RicochetActivatedAction ricochetActivatedAction;
        /**********************************************/

        /******************* INIT *********************/
        private void Awake()
        {
            projectileFactory = ProjectileFactory.Instance;
            body = GetComponent<Rigidbody2D>();
            audioSource = GetComponent<AudioSource>();

            Node = new LinkedListNode<GameObject>(gameObject);
            healthBar = GetComponent<HealthBar>();
            healthBar.AddFirst(Node);

            healthPickupAction = () => healthBar.SpawnSegment();
            speedMultiplierAction =
                (duration, multiplier) => StartCoroutine(ApplySpeedMultiplier(duration, multiplier));
            ricochetActivatedAction = duration => StartCoroutine(ApplyRicochet(duration));

            GameManager.OnHealthPickup += healthPickupAction;
            GameManager.OnSpeedMultiplierApplied += speedMultiplierAction;
            GameManager.OnRicochetActivated += ricochetActivatedAction;
        }

        private void Start()
        {
            for (int i = 0; i < startingHealthAmount; i++)
            {
                healthBar.SpawnSegment();
            }
        }

        private void OnDestroy()
        {
            GameManager.OnHealthPickup -= healthPickupAction;
            GameManager.OnSpeedMultiplierApplied -= speedMultiplierAction;
            GameManager.OnRicochetActivated -= ricochetActivatedAction;
        }
        /**********************************************/

        /******************* LOOP *********************/
        private void Update()
        {
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
        }

        private void FixedUpdate()
        {
            velocity = body.velocity;

            float acceleration = lastVelocity.magnitude < velocity.magnitude ? maxAcceleration : maxDeceleration;
            float maxSpeedChange = acceleration * speedMultiplier * Time.deltaTime;
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
            projectileFactory.Instantiate(projectileType, projectilePosition, shootDirection);
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

        private void DetachHealthBar()
        {
            if (healthBar.IsFirst(Node)) healthBar.RemoveFirst();
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
            else if (collision.gameObject.CompareTag("HealthBar") && !healthBar.IsFirst(Node))
            {
                healthBar.AddFirst(Node);
            }
        }

        public void OnProjectileHit(Projectile projectile)
        {
            OnDamage();
        }

        private void OnDamage()
        {
            damageSounds.Play(audioSource);

            if (!healthBar.IsFirst(Node) || healthBar.Count <= 1)
            {
                deathSounds.Play(audioSource);
                GameManager.Instance.Death();
            }
            else
            {
                healthBar.RemoveLast();
            }
        }
        /**********************************************/
    }
}