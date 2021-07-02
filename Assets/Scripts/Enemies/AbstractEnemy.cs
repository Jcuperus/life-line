using Spine;
using Spine.Unity;
using UnityEngine;
using Gameplay.Projectile;

/// <summary>
/// Base class from which all enemy monobehaviours should be derived.
/// </summary>
public abstract class AbstractEnemy : MonoBehaviour, IProjectileHit
{
    /**************** VARIABLES *******************/
    [Header("Stats")]
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected int health = 15;
    [Space]
    
    [Header("Audio")]
    [SerializeField] protected AudioEvent hitSound;
    [SerializeField] protected AudioEvent fireSound;
    [SerializeField] protected AudioEvent deathSound;
    
    [Header("Animation")]
    [SerializeField] protected SkeletonAnimation animator;

    protected AudioSource audioSource;
    protected Vector2 moveDirection;
    protected Rigidbody2D body;
    protected PlayerMovement player;
    
    protected AnimationState currentState;
    protected bool canInterruptAnimation = true;

    protected enum AnimationState
    {
        Idle,
        Hurt,
        Attacking,
        Death
    }
    /**********************************************/
    
    /******************* INIT *********************/
    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();
    }
    /**********************************************/
    
    /***************** METHODS ********************/
    protected void SetAnimation(AnimationReferenceAsset animation, bool loop, float timeScale)
    {
        if (!canInterruptAnimation) return;
        
        TrackEntry trackEntry = animator.state.SetAnimation(0, animation, loop);
        trackEntry.TimeScale = timeScale;
        trackEntry.Complete += OnAnimationComplete;
    }
    
    protected void DestroyEnemy()
    {
        StopAllCoroutines();
        GameManager.Instance.EnemiesAlive++;
        Destroy(gameObject);
    }
    
    protected virtual void OnAnimationComplete(TrackEntry trackEntry) {}
    
    public virtual void OnProjectileHit(Projectile projectile)
    {
        currentState = AnimationState.Hurt;
        hitSound.Play(audioSource);
        health--;
        
        if (health < 1)
        {
            currentState = AnimationState.Death;
            deathSound.Play(audioSource);
        }
    }
    /**********************************************/
}
