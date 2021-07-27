using System;
using Animation;
using Gameplay;
using Player;
using UnityEngine;
using Utility;

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
        [SerializeField] protected float moveSpeed = 1f;
        [SerializeField] protected int health = 15;

        [Header("Audio")]
        [SerializeField] protected AudioEvent hitSound;
        [SerializeField] protected AudioEvent fireSound;
        [SerializeField] protected AudioEvent deathSound;

        [Header("Animation")]
        [SerializeField] protected ActorAnimationController animationController;

        private new Collider2D collider;
        
        protected AudioSource audioSource;
        protected Vector2 moveDirection;
        protected Rigidbody2D body;
        protected PlayerController player;
        protected bool isAlive = true;

        public static event Action OnEnemyIsDestroyed;
        /**********************************************/
    
        /******************* INIT *********************/
        protected virtual void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            collider = GetComponent<Collider2D>();
            audioSource = GetComponent<AudioSource>();
            player = FindObjectOfType<PlayerController>();

            animationController.OnDeathAnimationFinished += DestroyEnemy;
        }
        /**********************************************/
    
        /***************** METHODS ********************/
        protected virtual void DestroyEnemy()
        {
            OnEnemyIsDestroyed?.Invoke();
            Destroy(gameObject);
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
