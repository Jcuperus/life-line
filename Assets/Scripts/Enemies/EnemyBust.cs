using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

public class EnemyBust : AbstractEnemy
{
    [SerializeField] private Projectile projectilePreFab;
    [SerializeField] private float fireRate = 1f;
    
    [SerializeField] private AnimationReferenceAsset idleAnimation;
    [SerializeField] private AnimationReferenceAsset attackAnimation;
    [SerializeField] private AnimationReferenceAsset deathAnimation;
    
    private float timer;

    #region loop
    
    private void Start()
    {
        StartCoroutine(FireBulletCoroutine());
        currentState = AnimationState.Idle;
        SetAnimation(idleAnimation, true, 1f);
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1)
        {
            timer = 0;
            Vector3 target = player.transform.position;
            moveDirection = (target - transform.position).normalized;
        }
    }
    
    void FixedUpdate()
    {
        body.velocity += moveSpeed * Time.deltaTime * moveDirection;
    }
    #endregion

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
}
