using System.Collections;
using Spine;
using Spine.Unity;
using UnityEngine;
using Gameplay.Projectile;

public class EnemyBust : AbstractEnemy
{
    /**************** VARIABLES *******************/
    [SerializeField] private AnimationReferenceAsset idleAnimation;
    [SerializeField] private AnimationReferenceAsset attackAnimation;
    [SerializeField] private AnimationReferenceAsset deathAnimation;
    
    [Header("Projectile")]
    [SerializeField] private Projectile projectilePreFab;
    [SerializeField] private float fireRate = 1f;

    private float timer;
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
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1)
        {
            timer = 0;
            Vector3 target = player.transform.position;
            moveDirection = (target - transform.position).normalized;
        }
    }
    
    private void FixedUpdate()
    {
        body.velocity += moveSpeed * Time.deltaTime * moveDirection;
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
    
    private void FireBullet(Vector2 dir)
    {
        if (currentState == AnimationState.Death) return;
        
        fireSound.Play(audioSource);
        currentState = AnimationState.Attacking;
        SetAnimation(attackAnimation, false, 2f);
        
        Projectile projectile = Instantiate(projectilePreFab);
        projectile.transform.position = transform.position + (Vector3)dir * 2f;
        projectile.direction = dir;
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
