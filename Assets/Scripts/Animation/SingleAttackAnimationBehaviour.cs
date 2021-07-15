using Spine.Unity;
using UnityEngine;

namespace Animation
{
    [RequireComponent(typeof(ActorAnimationController))]
    public class SingleAttackAnimationBehaviour : AttackAnimationBehaviour
    {
        [SerializeField] private AnimationReferenceAsset attackAnimation;

        public override void Play(float timeScale = 1)
        {
            animationController.CurrentState = ActorAnimationController.AnimationState.Attacking;
            animationController.SetAnimation(attackAnimation, false, timeScale);
        }
    }
}