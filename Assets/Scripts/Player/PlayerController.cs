using System;
using System.Collections.Generic;
using Animation;
using Gameplay;
using UnityEngine;
using Utility;

namespace Player
{
    /// <summary>
    /// Behaviour script for player movement.
    /// </summary>
    [RequireComponent(typeof(MovementController), typeof(FireController), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IDamageable
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
        
        private AudioSource audioSource;
        private MovementController movementController;
        private FireController fireController;
        private new Collider2D collider;

        private HealthBar healthBar;
        private LinkedListNode<GameObject> Node { get; set; }
        
        private int healthBarLength;
        private bool isAlive = true;
        
        public delegate void DamageBoostChangeAction(int amount);
        public static event DamageBoostChangeAction OnDamageBoostChanged;

        public static event Action OnPlayerIsDamaged;
        /**********************************************/

        /******************* INIT *********************/
        private void Awake()
        {
            collider = GetComponent<Collider2D>();
            audioSource = GetComponent<AudioSource>();
            movementController = GetComponent<MovementController>();
            fireController = GetComponent<FireController>();

            Node = new LinkedListNode<GameObject>(gameObject);
        }
        
        private void OnEnable()
        {
            animationController.OnDeathAnimationFinished += () => GameManager.Instance.Death();
            GameManager.OnHealthPickup += SpawnHealthBarSegment;
            GameManager.OnStateChanged += OnStateChanged;
        }

        private void OnDisable()
        {
            GameManager.OnHealthPickup -= SpawnHealthBarSegment;
            GameManager.OnStateChanged -= OnStateChanged;
        }

        private void Start()
        {
            for (int i = 0; i < startingHealthAmount; i++)
            {
                SpawnHealthBarSegment();
            }
        }
        /**********************************************/

        /******************* LOOP *********************/
        private void Update()
        {
            if (GameManager.CurrentState == GameManager.State.Paused || !isAlive) return;

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
            OnDamageBoostChanged?.Invoke(0);
            healthBar.SpawnSegment();
            healthBarLength = healthBar.Count;
        }

        private void AttachHealthBar(HealthBar newHealthBar)
        {
            healthBar = newHealthBar;
            healthBar.AddFirst(Node);
            healthBarLength = healthBar.Count;
            OnDamageBoostChanged?.Invoke(0);
        }

        private void DetachHealthBar()
        {
            if (!HasHealthBar()) return;
            OnDamageBoostChanged?.Invoke(healthBarLength);
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
            if (!HasHealthBar() && collision.gameObject.TryGetComponent(out HealthBarSegment segment))
            {
                AttachHealthBar(segment.Parent);
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
            animationController.PlayDeathAnimation();
            SetReceivesInput(false);
            deathSounds.Play(audioSource);
        }

        private void OnStateChanged(GameManager.State state)
        {
            SetReceivesInput(state == GameManager.State.Running);
        }

        private void SetReceivesInput(bool receivesInput)
        {
            collider.enabled = receivesInput;
            movementController.enabled = receivesInput;
            fireController.enabled = receivesInput;
        }
        
        public void OnDamaged(int damage)
        {
            OnPlayerIsDamaged?.Invoke();
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
        /**********************************************/
    }
}