using UnityEngine;

namespace Gameplay.Projectile
{
    [CreateAssetMenu(fileName = nameof(ProjectileConfiguration), menuName = "ScriptableObjects/Projectile/ProjectileConfiguration", order = 1)]
    public class ProjectileConfiguration : ScriptableObject
    {
        public ProjectileAppearance appearance, ricochetAppearance;
        public float projectileSpeed;
        public bool playerIsOwner;
        public int ricochetAmount;
        public int damage;
    }
}