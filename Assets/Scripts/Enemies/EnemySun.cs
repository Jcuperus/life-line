using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;


public class EnemySun : AbstractEnemy
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float patternShootDelay = 5.5f;
    
    [SerializeField] private AnimationReferenceAsset idleAnimation;
    [SerializeField] private AnimationReferenceAsset attackAAnimation;
    [SerializeField] private AnimationReferenceAsset attackBAnimation;
    [SerializeField] private AnimationReferenceAsset attackCAnimation;
    [SerializeField] private AnimationReferenceAsset hurtAnimation;
    [SerializeField] private AnimationReferenceAsset deathAnimation;

    private FireBehaviour[] attackBehaviours;
    private AnimationReferenceAsset[] attackAnimations;

    protected override void Awake()
    {
        base.Awake();

        attackBehaviours = new FireBehaviour[]
        {
            new BulletStream(transform, projectilePrefab, player.transform),
            new BulletArc(transform, projectilePrefab, player.transform),
            new BulletCircle(transform, projectilePrefab)
        };

        attackAnimations = new[]
        {
            attackAAnimation, attackBAnimation, attackCAnimation
        };
    }

    protected override void OnAnimationComplete(TrackEntry trackEntry)
    {
        switch (currentState)
        {
            case AnimationState.Attacking:
            case AnimationState.Hurt:
                currentState = AnimationState.Idle;
                SetAnimation(idleAnimation, true, 1f);
                break;
            case AnimationState.Death:
                DestroyEnemy();
                GameManager.Instance.Victory();
                break;
        }
    }

    private void Start()
    {
        currentState = AnimationState.Idle;
        SetAnimation(idleAnimation, true, 1f);
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        int behaviourIndex = 0;

        while (gameObject.activeSelf)
        {
            Debug.Log("sun attack");
            FireBehaviour currentBehaviour = attackBehaviours[behaviourIndex];

            StartCoroutine(currentBehaviour.Execute());
            
            SetAnimation(attackAnimations[behaviourIndex], false, 1f);
            currentState = AnimationState.Attacking;
            
            behaviourIndex = Random.Range(0, attackBehaviours.Length);
            yield return new WaitForSeconds(patternShootDelay);
        }
    }

    public override void OnProjectileHit(Projectile projectile)
    {
        base.OnProjectileHit(projectile);

        if (currentState == AnimationState.Hurt)
        {
            SetAnimation(hurtAnimation, false, 1f);
        }
        if (currentState == AnimationState.Death)
        {
            Debug.Log("ded");
            SetAnimation(deathAnimation, false, 1f);
            canInterruptAnimation = false;
        }
    }
}
