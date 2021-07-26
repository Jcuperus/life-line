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
    [RequireComponent(typeof(MovementController), typeof(FireController))]
    public class PlayerController : MonoBehaviour, IProjectileHit
    {
        /**************** VARIABLES *******************/
        public ActorAnimationController AnimationController => animationController;

        [SerializeField] private ActorAnimationController animationController;
        [SerializeField] private HealthBar healthBarPrefab;
        [SerializeField] private int startingHealthAmount = 5;
        
        [SerializeField, Range(0f, 1f)]
        private float healthSegmentWeight = 0.05f;

        [Header("Sound Effects")] 
        [SerializeField] private AudioEvent damageSounds;
        [SerializeField] private AudioEvent deathSounds;

        private Rigidbody2D body;
        private AudioSource audioSource;
        private MovementController movementController;
        private FireController fireController;

        private HealthBar healthBar;
        private LinkedListNode<GameObject> Node { get; set; }
        
        private int healthBarLength;
        private bool isAlive = true;
        /**********************************************/

        /******************* INIT *********************/
        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            audioSource = GetComponent<AudioSource>();
            movementController = GetComponent<MovementController>();
            fireController = GetComponent<FireController>();

            Node = new LinkedListNode<GameObject>(gameObject);
            
            animationController.OnDeathAnimationFinished += () => GameManager.Instance.Death();
            GameManager.OnHealthPickup += SpawnHealthBarSegment;
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
        }
        /**********************************************/

        /******************* LOOP *********************/
        private void Update()
        {
            if (!isAlive) return;

            movementController.weightModifier = Mathf.Max(0.25f, 1f - healthSegmentWeight * healthBarLength);

            if (Input.GetButtonDown("Jump"))
            {
                DetachHealthBar();
            }
        }
        /**********************************************/

        /***************** METHODS ********************/
        private void SpawnHealthBarSegment()
        {
            if (healthBar == null)
            {
                healthBar = Instantiate(healthBarPrefab);
                healthBar.AddFirst(Node);
            }
            fireController.DamageBoost = 0;
            healthBar.SpawnSegment();
            healthBarLength = healthBar.Count;
        }

        private void AttachHealthBar(HealthBar newHealthBar)
        {
            healthBar = newHealthBar;
            healthBar.AddFirst(Node);
            healthBarLength = healthBar.Count;
            fireController.DamageBoost = 0;
        }

        private void DetachHealthBar()
        {
            if (!HasHealthBar()) return;
            fireController.DamageBoost = healthBarLength;
            healthBar.RemoveFirst();
            healthBar = null;
            healthBarLength = 0;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Pickup pickup))
            {
                GameManager.Instance.ResolvePickup(pickup);
                Destroy(pickup.gameObject);
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
            Camera.main.GetComponent<CameraMove>().Shake(.01f, .5f); // could probably be done more cleanly?
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
            movementController.enabled = false;
            fireController.enabled = false;
            deathSounds.Play(audioSource);
        }
        
        public void OnProjectileHit(Projectile projectile)
        {
            OnDamage();
        }
        /**********************************************/
    }
}