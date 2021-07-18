using UnityEngine;

namespace Gameplay.Projectile
{
    [CreateAssetMenu(fileName = "ProjectileConfiguration", menuName = "ScriptableObjects/ProjectileScriptableObject", order = 1)]
    public class ProjectileScriptableObject : ScriptableObject
    {
        public Sprite projectileSprite;
        public Gradient trailGradient;
        public float projectileSpeed;
        public bool playerIsOwner;
        public int startRicochet;
    }
}