using UnityEngine;

namespace Gameplay.Projectile
{
    [CreateAssetMenu(fileName = nameof(ProjectileAppearance), menuName = "ScriptableObjects/Projectile/ProjectileVisuals", order = 2)]
    public class ProjectileAppearance : ScriptableObject
    {
        public Sprite sprite;
        public Gradient trailGradient;
    }
}