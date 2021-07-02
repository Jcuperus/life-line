using Spine;
using Spine.Unity;
using UnityEngine;
using Gameplay.Projectile;

public class EnemyDoplhin : AbstractEnemy
{
    /**************** VARIABLES *******************/
    [SerializeField] private AnimationReferenceAsset attackAnimation;
    [SerializeField] private AnimationReferenceAsset deathAnimation;
    
    private float timer;
    /**********************************************/
    
    /******************* LOOP *********************/
    private void Update()
    {
        {
            timer += Time.deltaTime;
            if (timer > 1)
            {
                timer = 0;
                Vector3 target = player.transform.position;
                moveDirection = (target - transform.position).normalized;
            }
        }
    }
    
    private void FixedUpdate()
    {
        body.velocity += moveSpeed * moveSpeed * Time.deltaTime * moveDirection;
    }
    /**********************************************/
    
    /***************** METHODS ********************/
    protected override void OnAnimationComplete(TrackEntry trackEntry)
    {
        switch (currentState)
        {
            case AnimationState.Death:
                DestroyEnemy();
                break;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            fireSound.Play(audioSource);
            currentState = AnimationState.Attacking;
            SetAnimation(attackAnimation, false, 1f);
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
