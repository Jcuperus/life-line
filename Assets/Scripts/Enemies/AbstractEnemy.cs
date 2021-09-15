using System;
using System.Collections;
using Animation;
using Gameplay;
using Player;
using UnityEngine;
using Utility;
using Utility.Extensions;
using Random = UnityEngine.Random;

namespace Enemies
{
    /// <summary>
    /// Base class from which all enemy MonoBehaviours should be derived.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(AudioSource))]
    public abstract class AbstractEnemy : MonoBehaviour, IDamageable
    {
        /**************** VARIABLES *******************/
        [Header("Stats")]
        [SerializeField] protected int health = 15;
        [SerializeField] protected float minActivationDelay = 0.3f, maxActivationDelay = 10f;

        [Header("Audio")]
        [SerializeField] protected AudioEvent hitSound;
        [SerializeField] protected AudioEvent fireSound;
        [SerializeField] protected AudioEvent deathSound;

        [Header("Animation")]
        [SerializeField] protected ActorAnimationController animationController;

        private new Collider2D collider;
        
        protected MoveBehaviour moveBehaviour;
        protected AudioSource audioSource;
        protected PlayerController player;
        protected bool isAlive = true;
        private bool isActive;

        public static event Action OnEnemyIsDestroyed;
        /**********************************************/
    
        /******************* INIT *********************/
        protected virtual void Awake()
        {
            collider = GetComponent<Collider2D>();
            audioSource = GetComponent<AudioSource>();
            player = FindObjectOfType<PlayerController>();
            moveBehaviour = GetComponent<MoveBehaviour>();

            animationController.OnDeathAnimationFinished += DestroyEnemy;
            animationController.OnSpawnAnimationFinished += EnableEnemy;
            
        }

        protected virtual void Start()
        {
            DisableEnemy();
            StartCoroutine(AttackCoroutine());
        }
        /**********************************************/
    
        /***************** METHODS ********************/
        private void DisableEnemy()
        {
            if (moveBehaviour != null) moveBehaviour.enabled = false;
            isActive = false;

            if (animationController.HasSpawnAnimation())
            {
                animationController.PlaySpawnAnimation();
            }
            else
            {
                this.DelayedAction(EnableEnemy, Random.Range(minActivationDelay, maxActivationDelay));
            }
        }

        private void EnableEnemy()
        {
            if (moveBehaviour != null) moveBehaviour.enabled = true;
            isActive = true;
        }
        
        protected virtual void DestroyEnemy()
        {
            OnEnemyIsDestroyed?.Invoke();
            Destroy(gameObject);
        }

        protected virtual IEnumerator AttackCoroutine()
        {
            yield return new WaitUntil(() => isActive);
        }

        public virtual void OnDamaged(int damage)
        {
            if (!isAlive) return;
            
            animationController.PlayHurtAnimation();
            hitSound.Play(audioSource);
            health -= damage;

            if (health < 1)
            {
                animationController.PlayDeathAnimation();
                deathSound.Play(audioSource);
                collider.enabled = false;
                isAlive = false;
            }
        }
        /**********************************************/
    }
}
