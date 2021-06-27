using Spine;
using Spine.Unity;
using UnityEngine;

public abstract class AbstractEnemy : MonoBehaviour, ProjectileHit
{
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected int health = 15;
    
    [SerializeField] protected SkeletonAnimation animator;

    [SerializeField] protected AudioEven hitSound;
    [SerializeField] protected AudioEven fireSound;
    [SerializeField] protected AudioEven deathSound;
    
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

    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();
    }

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
        GameManager.Instance.enemiesAlive++;
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
    
    
}
