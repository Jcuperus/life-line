using Spine.Unity;
using UnityEngine;

namespace Animation
{
    [RequireComponent(typeof(SkeletonAnimation))]
    public class UnscaledSkeletonAnimation : MonoBehaviour
    {
        private SkeletonAnimation skeletonAnimation;
    
        private void Start()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
        }

        private void Update()
        {
            skeletonAnimation.Update(Time.unscaledDeltaTime);
            skeletonAnimation.LateUpdate();
        }
    }
}


