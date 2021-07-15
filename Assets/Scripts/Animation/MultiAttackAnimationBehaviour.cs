using Spine.Unity;
using UnityEngine;

namespace Animation
{
    public class MultiAttackAnimationBehaviour : AttackAnimationBehaviour
    {
        [SerializeField] private AnimationReferenceAsset[] attackAnimations;
        
        public override void Play(float timeScale = 1)
        {
            int animationIndex = Random.Range(0, attackAnimations.Length);
            animationController.CurrentState = ActorAnimationController.AnimationState.Attacking;
            animationController.SetAnimation(attackAnimations[animationIndex], false, timeScale);
        }

        public void Play(int animationIndex, float timeScale = 1)
        {
            animationController.CurrentState = ActorAnimationController.AnimationState.Attacking;
            animationController.SetAnimation(attackAnimations[animationIndex], false, timeScale);
        }
    }
}