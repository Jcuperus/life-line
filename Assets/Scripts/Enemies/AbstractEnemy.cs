using System;
using Animation;
using Gameplay.Projectile;
using Player;
using UnityEngine;
using Utility;

namespace Enemies
{
    /// <summary>
    /// Base class from which all enemy MonoBehaviours should be derived.
    /// </summary>
    public abstract class AbstractEnemy : MonoBehaviour, IProjectileHit
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

        protected AudioSource audioSource;
        protected Vector2 moveDirection;
        protected Rigidbody2D body;
        protected PlayerMovement player;
        protected bool isAlive = true;

        public static event Action OnEnemyIsDestroyed;
        /**********************************************/
    
        /******************* INIT *********************/
        protected virtual void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            player = FindObjectOfType<PlayerMovement>();
            audioSource = GetComponent<AudioSource>();

            animationController.OnDeathAnimationFinished += DestroyEnemy;
        }
        /**********************************************/
    
        /***************** METHODS ********************/
        protected virtual void DestroyEnemy()
        {
            StopAllCoroutines();
            OnEnemyIsDestroyed?.Invoke();
            Destroy(gameObject);
        }

        public virtual void OnProjectileHit(Projectile projectile)
        {
            if (!isAlive) return;
            
            animationController.PlayHurtAnimation();
            hitSound.Play(audioSource);
            health-= projectile.damage;

            if (health < 1)
            {
                animationController.PlayDeathAnimation();
                deathSound.Play(audioSource);
                isAlive = false;
            }
        }
        /**********************************************/
    }
}
