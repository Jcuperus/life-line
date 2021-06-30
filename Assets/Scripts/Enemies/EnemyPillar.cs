using System.Collections;
using Spine;
using Spine.Unity;
using UnityEngine;

public class EnemyPillar : AbstractEnemy
{
    /**************** VARIABLES *******************/
    [SerializeField] private AnimationReferenceAsset idleAnimation;
    [SerializeField] private AnimationReferenceAsset attackAnimation;
    [SerializeField] private AnimationReferenceAsset deathAnimation;
    [Header("Projectile")]
    [SerializeField] private Projectile projectilePreFab;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float moveDistance = 2f;
    
    private Vector3 target;
    /**********************************************/
    /******************* INIT *********************/
    private void Start()
    {
        StartCoroutine(FireBulletCoroutine());
        currentState = AnimationState.Idle;
        SetAnimation(idleAnimation, true, 1f);
    }
    /**********************************************/
    /******************* LOOP *********************/
    void Update()
    {
        if (moveDistance > 0)
        {
            target = player.transform.position;
            moveDirection = (target - transform.position).normalized;
            moveDistance -= Time.deltaTime;
        }
    }
    void FixedUpdate()
    {
        if (moveDistance > 0)
        {
            body.velocity += moveSpeed * Time.deltaTime * moveDirection;
        }
        else
        {
            body.velocity = Vector2.zero;
        }
    }
    /**********************************************/
    /***************** METHODS ********************/
    protected override void OnAnimationComplete(TrackEntry trackEntry)
    {
        switch (currentState)
        {
            case AnimationState.Attacking:
                currentState = AnimationState.Idle;
                SetAnimation(idleAnimation, true, 1f);
                break;
            case AnimationState.Death:
                DestroyEnemy();
                break;
        }
    }
    private void FireBullet(Vector2 direction)
    {
        if (currentState == AnimationState.Death) return;
        
        fireSound.Play(audioSource);
        currentState = AnimationState.Attacking;
        SetAnimation(attackAnimation, false, 3f);
        
        Projectile projectile = Instantiate(projectilePreFab);
        projectile.transform.position = transform.position + (Vector3) direction * 2f;
        projectile.direction = direction;
        projectile.ricochet = true;
    }
    private IEnumerator FireBulletCoroutine()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(fireRate);
            FireBullet(moveDirection);
        }
    }
    public override void OnProjectileHit(Projectile projectile)
    {
        base.OnProjectileHit(projectile);
        
        if (currentState == AnimationState.Death)
        {
            SetAnimation(deathAnimation, false, 1f);
            canInterruptAnimation = false;
        }
    }
    /**********************************************/
}
