using System.Collections;
using Spine;
using Spine.Unity;
using UnityEngine;
using Utility;

namespace Enemies
{
    public class EnemyWallSpawnPoint : EnemySpawnPoint
    {
        [Header("Animation")]
        [SerializeField] private SkeletonAnimation animator;
        [SerializeField] private AnimationReferenceAsset spawnAnimation;
        
        protected override IEnumerator SpawnEnemies(SubWave subWave)
        {
            SetAnimation(spawnAnimation, true, 0.5f);

            yield return base.SpawnEnemies(subWave);
            
            animator.ClearState();
        }
        
        private void SetAnimation(AnimationReferenceAsset animation, bool loop, float timeScale)
        {
            TrackEntry trackEntry = animator.state.SetAnimation(0, animation, loop);
            trackEntry.TimeScale = timeScale;
        }
    }
}